using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Web.Http.Filters;

namespace SpartanHub.Service
{
    public class UwpWebViewCookieReader : IHaloWaypointCookieReader
    {
        public const string SpartanTokenCookieName = "343-spartan-token";

        private static readonly Uri[] HaloWaypointCookieUris =
        {
            new Uri("https://www.halowaypoint.com"),
            new Uri("https://halowaypoint.com")
        };

        public Task<string> TryGetSpartanTokenAsync()
        {
            using (var filter = new HttpBaseProtocolFilter())
            {
                foreach (var uri in HaloWaypointCookieUris)
                {
                    var tokenCookie = filter.CookieManager
                        .GetCookies(uri)
                        .FirstOrDefault(cookie => string.Equals(cookie.Name, SpartanTokenCookieName, StringComparison.OrdinalIgnoreCase));

                    if (!string.IsNullOrWhiteSpace(tokenCookie?.Value))
                    {
                        return Task.FromResult(NormalizeCookieValue(tokenCookie.Value));
                    }
                }
            }

            return Task.FromResult<string>(null);
        }

        public Task ClearCookiesAsync()
        {
            using (var filter = new HttpBaseProtocolFilter())
            {
                foreach (var uri in HaloWaypointCookieUris)
                {
                    var cookies = filter.CookieManager.GetCookies(uri).ToArray();
                    foreach (var cookie in cookies)
                    {
                        filter.CookieManager.DeleteCookie(cookie);
                    }
                }
            }

            return Task.CompletedTask;
        }

        private static string NormalizeCookieValue(string value)
        {
            var token = value?.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(token))
            {
                return null;
            }

            try
            {
                return Uri.UnescapeDataString(token).Trim().Trim('"');
            }
            catch
            {
                return token;
            }
        }
    }
}
