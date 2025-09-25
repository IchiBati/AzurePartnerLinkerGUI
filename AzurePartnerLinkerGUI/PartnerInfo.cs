using System.Text.Json.Serialization;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Stellt die oberste Ebene der JSON-Antwort der Partner-Management-API dar.
    /// </summary>
    public record PartnerApiResponse
    {
        [JsonPropertyName("properties")]
        public PartnerInfo? Properties { get; set; }
    }

    /// <summary>
    /// Enthält die relevanten PAL-Informationen im "properties"-Objekt.
    /// </summary>
    public record PartnerInfo
    {
        /// <summary>
        /// Die Partner-ID.
        /// </summary>
        [JsonPropertyName("partnerId")]
        public string? PartnerId { get; set; }

        /// <summary>
        /// Der Name des Partners.
        /// </summary>
        [JsonPropertyName("partnerName")]
        public string? PartnerName { get; set; }

        /// <summary>
        /// Die Tenant-ID.
        /// </summary>
        [JsonPropertyName("tenantId")]
        public string? TenantId { get; set; }

        /// <summary>
        /// Der Status des Partners.
        /// </summary>
        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}
