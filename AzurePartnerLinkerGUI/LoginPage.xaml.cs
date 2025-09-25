using System;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ManagementPartner;



namespace AzurePartnerLinkerGUI
{
    public partial class LoginPage : Page
    {
        
        public event EventHandler<LoginSuccessEventArgs> AuthenticationSuccess;
        private readonly ILogger _logger;
        
        public LoginPage(ILogger logger)
        {
            InitializeComponent();
            _logger = logger;
        }
        
        private void Authtenticate(ClientSecretCredential credential)
        {
            _logger.Log("Attempting to authenticate with Azure...");
            
            
        }

        private async Task<bool> IsAuthenticated(ArmClient armClient)
        {
            _logger.Log("Attempting to authenticate with Azure...");
            AuthenticateButton.IsEnabled = false;
            
            try
            {
                await foreach (var tenant in armClient.GetTenants().GetAllAsync())
                {
                    _logger.Log($"Validation successful. Tenant ID: {tenant.Data.TenantId}");
                    return true; 
                }
            }
            catch (AuthenticationFailedException authEx)
            {
                _logger.Log($"Authentication failed {authEx.Message}");
                return false;
            }
         
            return false;   
        }
        
        

        private async void Authenticate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var credential = new ClientSecretCredential(
                    TenantIdBox.Text,
                    ClientIdBox.Text,
                    ClientSecretBox.Password
                );

                var client = new ArmClient(credential);

                if (await IsAuthenticated(client))
                {
                    _logger.Log("Authentication successful");
                    
                    var partnerClient = new PartnerManagementClient(credential, _logger);
                    PartnerInfo? partnerInfo = await partnerClient.GetPartnerInfoAsync();
                    var eventArgs = new LoginSuccessEventArgs(client, TenantIdBox.Text, credential, partnerInfo);
                    AuthenticationSuccess?.Invoke(this, eventArgs);
                }

                // bearer holen! 
                var tokenRequestContext = new TokenRequestContext(new[] { "https://management.azure.com/.default" });
                AccessToken token = await credential.GetTokenAsync(tokenRequestContext);
                Console.WriteLine(token.Token);



                //PartnerResponseResource partnerResource = await client.GetPartnerResponseResource(new ResourceIdentifier($"/providers/Microsoft.ManagementPartner/partners/{PartnerIdBox.Text}")).GetAsync();
                //string? partnerId = partnerResource.Data.PartnerId;
                //_logger.Log($"Existing PAL found for Partner ID: {partnerId}");

                //await partnerResource.CreateOrUpdateAsync();

            }
            catch (Exception ex)
            {
                // Fängt andere mögliche Fehler ab (z.B. wenn der Partner nicht gefunden wird)
                _logger.Log($"An error occurred: {ex.Message}");
            }
            finally
            {
                AuthenticateButton.IsEnabled = true;
            }
        }


        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            ValidateInputs();
        }

        private void OnPasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            if (!this.IsLoaded)
            {
                return;
            }

            bool allFieldsFilled = !string.IsNullOrEmpty(ClientIdBox.Text) &&
                                   !string.IsNullOrEmpty(TenantIdBox.Text) &&
                                   !string.IsNullOrEmpty(ClientSecretBox.Password);

            AuthenticateButton.IsEnabled = allFieldsFilled;
        }

        
    }
}
