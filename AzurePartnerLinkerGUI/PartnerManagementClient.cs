using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using Azure.Core;
using Azure.Identity;

namespace AzurePartnerLinkerGUI
{
    /// <summary>
    /// Ein robuster Client, der sich um die Partner Admin Link (PAL) Operationen kümmert.
    /// </summary>
    public class PartnerManagementClient
    {
        private readonly ClientSecretCredential _credential;
        private readonly ILogger _logger;
        private const string ApiVersion = "2018-02-01";
        private const string ManagementApiScope = "https://management.azure.com/.default";
        private const string BaseUrl = "https://management.azure.com/providers/Microsoft.ManagementPartner/partners";

        public PartnerManagementClient(ClientSecretCredential credential, ILogger logger)
        {
            _credential = credential ?? throw new ArgumentNullException(nameof(credential));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Holt die vollständigen PAL-Informationen.
        /// </summary>
        /// <returns>Ein PartnerInfo-Objekt bei Erfolg, sonst null.</returns>
        public async Task<PartnerInfo?> GetPartnerInfoAsync()
        {
            var response = await SendRequestAsync(HttpMethod.Get, $"{BaseUrl}?api-version={ApiVersion}");
            if (response == null) return null;

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonSerializer.Deserialize<PartnerApiResponse>(jsonResponse);
                // _logger.Log($"Gefundener PAL: {apiResponse?.Properties?.PartnerId} ({apiResponse?.Properties?.PartnerName})");
                return apiResponse?.Properties;
            }

            // Wenn nicht gefunden, ist das kein Fehler, sondern bedeutet nur, dass kein Link existiert.
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // _logger.Log("Kein bestehender PAL-Link gefunden.");
                return null;
            }

            // Für alle anderen Fehler den allgemeinen Fehlerhandler verwenden.
            await HandleFailedResponseAsync(response);
            return null;
        }

        /// <summary>
        /// Erstellt oder aktualisiert den Partner Admin Link.
        /// </summary>
        public async Task<bool> LinkOrUpdatePalAsync(string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId)) return false;
            string url = $"{BaseUrl}/{partnerId}?api-version={ApiVersion}";
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await SendRequestAsync(HttpMethod.Put, url, content);

            if (response?.IsSuccessStatusCode ?? false)
            {
                _logger.Log($"PAL erfolgreich verknüpft/aktualisiert für Partner-ID: {partnerId}");
                return true;
            }

            await HandleFailedResponseAsync(response);
            return false;
        }

        /// <summary>
        /// Aktualisiert den PAL mit PATCH.
        /// </summary>
        public async Task<bool> PatchPalAsync(string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId)) return false;

            string url = $"{BaseUrl}/{partnerId}?api-version={ApiVersion}";
            // Für PATCH verwenden wir HttpMethod.Patch. Der Body ist laut API-Dokumentation oft derselbe.
            var content = new StringContent("{}", Encoding.UTF8, "application/json");
            var response = await SendRequestAsync(new HttpMethod("PATCH"), url, content);

            if (response?.IsSuccessStatusCode ?? false)
            {
                _logger.Log($"PAL erfolgreich aktualisiert für Partner-ID: {partnerId}");
                return true;
            }

            await HandleFailedResponseAsync(response);
            return false;
        }

        /// <summary>
        /// Löscht den Partner Admin Link.
        /// </summary>
        public async Task<bool> DeletePalAsync(string partnerId)
        {
            if (string.IsNullOrWhiteSpace(partnerId)) return false;
            string url = $"{BaseUrl}/{partnerId}?api-version={ApiVersion}";
            var response = await SendRequestAsync(HttpMethod.Delete, url);

            if (response?.IsSuccessStatusCode ?? false)
            {
                _logger.Log($"PAL-Link erfolgreich gelöscht für Partner-ID: {partnerId}");
                return true;
            }

            await HandleFailedResponseAsync(response);
            return false;
        }

        /// <summary>
        /// Zentraler Fehlerhandler für alle fehlgeschlagenen API-Antworten.
        /// Er liest die Standard-Fehler-JSON und zeigt eine verständliche Meldung an.
        /// </summary>
        private async Task HandleFailedResponseAsync(HttpResponseMessage? response)
        {
            if (response == null)
            {
                _logger.Log("Operation fehlgeschlagen: Keine Antwort vom Server.");
                return;
            }

            string errorMessage = $"Ein unbekannter Fehler ist aufgetreten (Statuscode: {response.StatusCode}).";
            try
            {
                string errorJson = await response.Content.ReadAsStringAsync();
                var errorResponse = JsonSerializer.Deserialize<ApiErrorResponse>(errorJson);
                if (!string.IsNullOrWhiteSpace(errorResponse?.Error?.Message))
                {
                    errorMessage = errorResponse.Error.Message;
                }
            }
            catch
            {
                // Falls das Parsen des Fehlers fehlschlägt, verwenden wir die generische Meldung.
            }

            _logger.Log($"API-Fehler: {errorMessage}");
        }

        private async Task<HttpResponseMessage?> SendRequestAsync(HttpMethod method, string url, HttpContent? content = null)
        {
            try
            {
                using var httpClient = await CreateAuthenticatedHttpClientAsync();
                using var request = new HttpRequestMessage(method, url);
                if (content != null)
                {
                    request.Content = content;
                }
                return await httpClient.SendAsync(request);
            }
            catch (Exception ex)
            {
                _logger.Log($"API-Anfrage '{method} {url}' fehlgeschlagen: {ex.Message}");
                return null;
            }
        }

        private async Task<HttpClient> CreateAuthenticatedHttpClientAsync()
        {
            var tokenRequestContext = new TokenRequestContext(new[] { ManagementApiScope });
            AccessToken token = await _credential.GetTokenAsync(tokenRequestContext);

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
            return httpClient;
        }
    }
}
