using System;
using System.Windows;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Hauptfenster der Anwendung, das auch als Logger fungiert.
    /// </summary>
    public partial class MainWindow : Window, ILogger
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLoginPage();
        }

        /// <summary>
        /// Protokolliert eine Nachricht in der Log-Liste.
        /// </summary>
        /// <param name="message">Die zu protokollierende Nachricht.</param>
        public void Log(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return;
            }

            string logEntry = $"{DateTime.Now:HH:mm:ss}: {message}";
            LogListBox.Items.Add(logEntry);
            if (LogListBox.Items.Count > 0)
            {
                LogListBox.ScrollIntoView(LogListBox.Items[LogListBox.Items.Count - 1]);
            }
        }

        /// <summary>
        /// Zeigt die Login-Seite im Hauptframe an.
        /// </summary>
        private void ShowLoginPage()
        {
            var loginPage = new LoginPage(this);

            // Lauscht auf das "Erfolgreich"-Signal von der LoginPage.
            loginPage.AuthenticationSuccess += LoginPage_AuthenticationSuccess;

            // Navigiert zum LoginPage-Frame.
            MainFrame.Navigate(loginPage);
        }

        /// <summary>
        /// Wird aufgerufen, wenn die LoginPage das erfolgreiche Login signalisiert.
        /// </summary>
        private void LoginPage_AuthenticationSuccess(object? sender, LoginSuccessEventArgs e)
        {
            // Zeigt das Dashboard nach erfolgreichem Login.
            ShowDashboard(e.AuthenticatedClient, e.TenantId, e.Credential, e.PartnerInfo);
        }

        /// <summary>
        /// Zeigt die Dashboard-Seite mit den übergebenen Parametern an.
        /// </summary>
        public void ShowDashboard(ArmClient client, string tenantId, ClientSecretCredential credential, PartnerInfo partnerInfo)
        {
            var dashboardPage = new DashboardPage(this, client, tenantId, credential, partnerInfo);
            MainFrame.Navigate(dashboardPage);
        }
    }
}
