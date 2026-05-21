using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpartanHub.Core.Exceptions;

namespace SpartanHub.Core.Authentication
{
    public enum RelyingParty
    {
        Xbox,
        Halo
    }

    public class XboxAuthenticationClient
    {
        private readonly Func<Task<string>> _getOauth2AccessToken;
        private readonly HttpClient _httpClient;

        private CachedUserToken _userToken;
        private CachedXstsTicket _xstsTicket;

        public XboxAuthenticationClient(Func<Task<string>> getOauth2AccessToken, HttpClient httpClient = null)
        {
            _getOauth2AccessToken = getOauth2AccessToken ?? throw new ArgumentNullException(nameof(getOauth2AccessToken));
            _httpClient = httpClient ?? new HttpClient();
        }

        public async Task<string> GetXstsTicketAsync(RelyingParty relyingParty)
        {
            if (_xstsTicket != null && _xstsTicket.RelyingParty == relyingParty && _xstsTicket.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(5))
            {
                return _xstsTicket.Token;
            }

            var userToken = await GetUserTokenAsync().ConfigureAwait(false);
            var relyingPartyUrl = relyingParty == RelyingParty.Halo ? "https://prod.xsts.halowaypoint.com/" : "http://xboxlive.com";

            var requestBody = new
            {
                RelyingParty = relyingPartyUrl,
                TokenType = "JWT",
                Properties = new
                {
                    SandboxId = "RETAIL",
                    UserTokens = new[] { userToken }
                }
            };

            var url = "https://xsts.auth.xboxlive.com/xsts/authorize";
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("x-xbl-contract-version", "1");

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HaloApiException(url, response);
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var ticket = JsonConvert.DeserializeObject<XboxTicket>(responseContent);

            _xstsTicket = new CachedXstsTicket
            {
                Token = ticket.Token,
                DisplayClaims = ticket.DisplayClaims,
                ExpiresAt = DateTimeOffset.Parse(ticket.NotAfter),
                RelyingParty = relyingParty
            };

            return _xstsTicket.Token;
        }

        public async Task ClearXstsTicketAsync(RelyingParty relyingParty)
        {
            if (_xstsTicket?.RelyingParty == relyingParty)
            {
                _xstsTicket = null;
            }
            _userToken = null;
        }

        public string GetXboxLiveV3Token(XboxTicket xboxTicket)
        {
            var uhs = xboxTicket.DisplayClaims?.xui?[0]?.uhs;
            return $"XBL3.0 x={uhs};{xboxTicket.Token}";
        }

        private async Task<string> GetUserTokenAsync()
        {
            if (_userToken != null && _userToken.ExpiresAt > DateTimeOffset.UtcNow.AddMinutes(5))
            {
                return _userToken.Token;
            }

            var oauthToken = await _getOauth2AccessToken().ConfigureAwait(false);

            var requestBody = new
            {
                RelyingParty = "http://auth.xboxlive.com",
                TokenType = "JWT",
                Properties = new
                {
                    AuthMethod = "RPS",
                    SiteName = "user.auth.xboxlive.com",
                    RpsTicket = $"d={oauthToken}"
                }
            };

            var url = "https://user.auth.xboxlive.com/user/authenticate";
            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };

            request.Headers.Add("Accept", "application/json");
            request.Headers.Add("x-xbl-contract-version", "1");

            var response = await _httpClient.SendAsync(request).ConfigureAwait(false);

            if (!response.IsSuccessStatusCode)
            {
                throw new HaloApiException(url, response);
            }

            var responseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            var ticket = JsonConvert.DeserializeObject<XboxTicket>(responseContent);

            _userToken = new CachedUserToken
            {
                Token = ticket.Token,
                ExpiresAt = DateTimeOffset.Parse(ticket.NotAfter)
            };

            return _userToken.Token;
        }
    }

    public class XboxTicket
    {
        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("NotAfter")]
        public string NotAfter { get; set; }

        [JsonProperty("DisplayClaims")]
        public DisplayClaims DisplayClaims { get; set; }
    }

    public class DisplayClaims
    {
        [JsonProperty("xui")]
        public XuiItem[] xui { get; set; }
    }

    public class XuiItem
    {
        [JsonProperty("uhs")]
        public string uhs { get; set; }
    }

    internal class CachedUserToken
    {
        public string Token { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
    }

    internal class CachedXstsTicket
    {
        public string Token { get; set; }
        public DisplayClaims DisplayClaims { get; set; }
        public DateTimeOffset ExpiresAt { get; set; }
        public RelyingParty RelyingParty { get; set; }
    }
}
