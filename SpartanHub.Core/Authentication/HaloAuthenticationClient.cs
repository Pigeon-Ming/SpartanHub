using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpartanHub.Core.Endpoints;
using SpartanHub.Core.Models;
using SpartanHub.Core.Exceptions;

namespace SpartanHub.Core.Authentication
{
    public class HaloAuthenticationClient : ISpartanTokenProvider
    {
        private readonly IXstsTokenProvider _xstsProvider;
        private readonly HttpClient _httpClient;

        private CachedToken _cachedToken;

        public HaloAuthenticationClient(IXstsTokenProvider xstsProvider, HttpClient httpClient = null)
        {
            _xstsProvider = xstsProvider ?? throw new ArgumentNullException(nameof(xstsProvider));
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> GetSpartanTokenAsync()
        {
            if (_cachedToken != null && _cachedToken.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(5))
            {
                return _cachedToken.Token;
            }

            await RefreshSpartanTokenAsync().ConfigureAwait(false);
            return _cachedToken.Token;
        }

        public async Task ClearSpartanTokenAsync()
        {
            _cachedToken = null;
            await _xstsProvider.ClearXstsTokenAsync().ConfigureAwait(false);
        }

        private async Task RefreshSpartanTokenAsync()
        {
            var xstsToken = await _xstsProvider.FetchTokenAsync().ConfigureAwait(false);

            var tokenRequest = new SpartanTokenRequest
            {
                Audience = "urn:343:s3:services",
                MinVersion = "4",
                Proof = new[]
                {
                    new ProofItem
                    {
                        Token = xstsToken,
                        TokenType = "Xbox_XSTSv3"
                    }
                }
            };

            var url = $"https://{HaloCoreEndpoints.SettingsOrigin}.{HaloCoreEndpoints.ServiceDomain}/spartan-token";
            var content = new StringContent(JsonConvert.SerializeObject(tokenRequest), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            request.Headers.Add("User-Agent", GlobalConstants.HaloWaypointUserAgent);
            request.Headers.Add("Accept", "application/json, text/plain, */*");

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HaloApiException(url, response);
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var spartanToken = JsonConvert.DeserializeObject<SpartanToken>(responseContent);

            _cachedToken = new CachedToken
            {
                Token = spartanToken.SpartanTokenValue,
                ExpiresAt = DateTimeOffset.Parse(spartanToken.ExpiresUtc.ISO8601Date)
            };
        }
    }

    internal class CachedToken
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }
}
