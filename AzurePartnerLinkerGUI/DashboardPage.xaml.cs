using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzurePartnerLinkerGUI
{
    public partial class DashboardPage : Page
    {

        private readonly ILogger _Dashlogger;
        private readonly PartnerManagementClient _partnerClient;
        private PartnerInfo? _currentPartnerInfo;

        public DashboardPage(ILogger _logger, ArmClient client, string tenantId, ClientSecretCredential credential, PartnerInfo partnerInfo)
        {
            InitializeComponent();
            _Dashlogger = _logger;
            TenantIdTextBlock.Text = tenantId;
            

            _partnerClient = new PartnerManagementClient(credential, _logger);
            _currentPartnerInfo = partnerInfo;
            UpdateStatusDisplay(_currentPartnerInfo);
            UpdateButtonStates();
        }

        private void UpdateButtonStates()
        {
            // Link/Update-Button aktivieren, wenn eine Partner-ID eingegeben wurde.
            bool isActionIdEntered = !string.IsNullOrWhiteSpace(PartnerIdBox.Text);
            LinkButton.IsEnabled = isActionIdEntered;

            // Löschen-Button aktivieren, wenn eine Partner-ID eingegeben wurde und ein PAL-Link existiert.
            DeleteButton.IsEnabled = isActionIdEntered && (_currentPartnerInfo != null);
        }

        private void UpdateStatusDisplay(PartnerInfo? partnerInfo)
        {
            if (partnerInfo != null)
            {
                PartnerIdTextBlock.Text = partnerInfo.PartnerId ?? "N/A";
                PartnerNameTextBlock.Text = partnerInfo.PartnerName ?? "N/A";
                PartnerIdTextBlock.Foreground = Brushes.Green;
                PartnerNameTextBlock.Foreground = Brushes.Green;
            }
            else
            {
                PartnerIdTextBlock.Text = "Keine Verknüpfung gefunden";
                PartnerNameTextBlock.Text = "---";
                PartnerIdTextBlock.Foreground = Brushes.OrangeRed;
            }
        }

        public void OnPartnerIdChanged(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateButtonStates();
        }

        private async void LinkPal_Click(object sender, RoutedEventArgs e)
        {
            LinkButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            bool success = false;

            // FRISCHE DATEN HOLEN, um die Entscheidung PUT vs. PATCH zu treffen
            _currentPartnerInfo = await _partnerClient.GetPartnerInfoAsync();

            if (_currentPartnerInfo == null)
            {
                // Fall A: Kein PAL da -> PUT (Erstellen)
                _Dashlogger.Log("No existing PAL found. Creating new link...");
                success = await _partnerClient.LinkOrUpdatePalAsync(PartnerIdBox.Text); // Hier müsste die PUT-Logik hin
            }
            else
            {
                // Fall B: PAL da -> PATCH (Aktualisieren)
                _Dashlogger.Log("Existing PAL found. Updating link...");
                success = await _partnerClient.PatchPalAsync(PartnerIdBox.Text);
            }

            // Nach der Aktion: Den Zustand der Welt neu abfragen und die UI aktualisieren
            _currentPartnerInfo = await _partnerClient.GetPartnerInfoAsync();
            UpdateStatusDisplay(_currentPartnerInfo);
            UpdateButtonStates();
        }

        private async void DeletePal_Click(object sender, RoutedEventArgs e)
        {
            LinkButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;

            // FRISCHE DATEN HOLEN, um sicherzugehen, dass es noch etwas zu löschen gibt
            _currentPartnerInfo = await _partnerClient.GetPartnerInfoAsync();
            _Dashlogger.Log("Deleting PAL link...");

            if (_currentPartnerInfo != null)
            {
                bool success = await _partnerClient.DeletePalAsync(PartnerIdBox.Text);
                if (success)
                {
                    // Doppelte Protokollierung, eventuell überarbeiten.
                    //_Dashlogger.Log("PAL-Link erfolgreich gelöscht.");
                }
            }
            else
            {
                _Dashlogger.Log("Löschvorgang abgebrochen. PAL-Link wurde bereits entfernt.");
            }

            // Nach der Aktion: Zustand neu abfragen und UI aktualisieren
            _currentPartnerInfo = await _partnerClient.GetPartnerInfoAsync();
            UpdateStatusDisplay(_currentPartnerInfo);
            UpdateButtonStates();




        }
    }
}
