using System;
using Azure.Identity;
using Azure.ResourceManager;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Ein benutzerdefinierter EventArgs-Datenträger, der beim erfolgreichen Login
    /// die nötigen Informationen von der LoginPage zum MainWindow übergibt.
    /// </summary>
    public class LoginSuccessEventArgs : EventArgs
    {
        /// <summary>
        /// Der authentifizierte ArmClient.
        /// </summary>
        public ArmClient AuthenticatedClient { get; }

        /// <summary>
        /// Die verwendeten Anmeldeinformationen.
        /// </summary>
        public ClientSecretCredential Credential { get; }

        /// <summary>
        /// Die Tenant-ID des angemeldeten Benutzers.
        /// </summary>
        public string TenantId { get; }

        /// <summary>
        /// Informationen zum Partner, falls vorhanden.
        /// </summary>
        public PartnerInfo? PartnerInfo { get; }

        public LoginSuccessEventArgs(ArmClient client, string tenantId, ClientSecretCredential credential, PartnerInfo? partnerInfo)
        {
            AuthenticatedClient = client;
            TenantId = tenantId;
            Credential = credential;
            PartnerInfo = partnerInfo;
        }
    }
}
