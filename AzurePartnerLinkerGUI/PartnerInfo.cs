using System.Text.Json.Serialization;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Repräsentiert die oberste Ebene der JSON-Antwort von der Partner-Management-API.
    /// </summary>
    public record PartnerApiResponse
    {
        [JsonPropertyName("properties")]
        public PartnerInfo? Properties { get; set; }
    }

    /// <summary>
    /// Repräsentiert das nützliche "properties"-Objekt, das alle relevanten PAL-Informationen enthält.
    /// </summary>
    public record PartnerInfo
    {
        [JsonPropertyName("partnerId")]
        public string? PartnerId { get; set; }

        [JsonPropertyName("partnerName")]
        public string? PartnerName { get; set; }

        [JsonPropertyName("tenantId")]
        public string? TenantId { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}