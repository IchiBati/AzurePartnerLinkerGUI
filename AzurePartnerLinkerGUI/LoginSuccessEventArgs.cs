using System;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Ein benutzerdefinierter "Datenbehälter" (EventArgs), der beim erfolgreichen Login
    /// die notwendigen Informationen von der LoginPage zum MainWindow transportiert.
    /// </summary>
    public class LoginSuccessEventArgs : EventArgs
    {
       
        public ArmClient AuthenticatedClient { get; }
        
        public ClientSecretCredential Credential { get; }
        public string TenantId { get; }
        
        public PartnerInfo? PartnerInfo { get; }
        
        public LoginSuccessEventArgs(ArmClient client, string tenantId,  ClientSecretCredential credential, PartnerInfo? partnerInfo)
        {
            AuthenticatedClient = client;
            TenantId = tenantId;
            Credential = credential;
            PartnerInfo = partnerInfo;
        }
    }
}