using System;
using System.Windows;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzurePartnerLinkerGUI
{
    public partial class MainWindow : Window, ILogger
    {
        public MainWindow()
        {
            InitializeComponent();
            ShowLoginPage();
        }

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

        private void ShowLoginPage()
        {
            var loginPage = new LoginPage(this);

            // Hier lauschen wir auf das "Erfolgreich"-Signal von der LoginPage
            loginPage.AuthenticationSuccess += LoginPage_AuthenticationSuccess;

            // Sagt dem Frame, er soll die Login-Seite laden
            MainFrame.Navigate(loginPage);
        }

        // Diese Methode wird aufgerufen, WENN das Signal von der LoginPage kommt
        private void LoginPage_AuthenticationSuccess(object? sender, LoginSuccessEventArgs e)
        {
            // Wenn der Login erfolgreich war, zeige das Dashboard
            ShowDashboard(e.AuthenticatedClient, e.TenantId, e.Credential, e.PartnerInfo);
        }

        public void ShowDashboard(ArmClient client, string tenantId, ClientSecretCredential credential, PartnerInfo partnerInfo)
        {
            var dashboardPage = new DashboardPage(this, client, tenantId, credential, partnerInfo);
            MainFrame.Navigate(dashboardPage);
        }
    }
}