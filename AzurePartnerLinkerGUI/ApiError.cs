using System.Text.Json.Serialization;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Stellt die Struktur einer Standard-Fehlerantwort der Azure-API dar.
    /// </summary>
    public record ApiErrorResponse
    {
        [JsonPropertyName("error")]
        public ApiError? Error { get; set; }
    }

    /// <summary>
    /// Stellt das innere 'error'-Objekt mit Code und Nachricht dar.
    /// </summary>
    public record ApiError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}
