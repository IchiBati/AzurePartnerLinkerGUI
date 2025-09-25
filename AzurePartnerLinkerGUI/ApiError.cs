using System.Text.Json.Serialization;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Represents the structure of a standard error response from the Azure API.
    /// </summary>
    public record ApiErrorResponse
    {
        [JsonPropertyName("error")]
        public ApiError? Error { get; set; }
    }
    
    /// <summary>
    /// Represents the inner 'error' object with code and message.
    /// </summary>
    public record ApiError
    {
        [JsonPropertyName("code")]
        public string? Code { get; set; }

        [JsonPropertyName("message")]
        public string? Message { get; set; }
    }
}