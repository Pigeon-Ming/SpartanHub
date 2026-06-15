using System;
using System.Linq;
using System.Threading.Tasks;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;

namespace SpartanHub.Service
{
    public interface IHaloWaypointLoginService
    {
        Task<LoginResult> CompleteLoginAsync();
        Task<LoginResult> SignInWithSpartanTokenAsync(string spartanToken);
        Task LogoutAsync();
    }

    public class HaloWaypointLoginService : IHaloWaypointLoginService
    {
        private readonly IHaloWaypointCookieReader _cookieReader;
        private readonly IPersistedSpartanSessionStore _sessionStore;

        public HaloWaypointLoginService(
            IHaloWaypointCookieReader cookieReader = null,
            IPersistedSpartanSessionStore sessionStore = null)
        {
            _cookieReader = cookieReader ?? new UwpWebViewCookieReader();
            _sessionStore = sessionStore ?? new PasswordVaultSpartanSessionStore();
        }

        public async Task<LoginResult> CompleteLoginAsync()
        {
            var token = await _cookieReader.TryGetSpartanTokenAsync().ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(token))
            {
                return LoginResult.Failed("未能从 HaloWaypoint Cookie 中读取 343-spartan-token。");
            }

            return await SignInWithSpartanTokenAsync(token).ConfigureAwait(false);
        }

        public async Task<LoginResult> SignInWithSpartanTokenAsync(string spartanToken)
        {
            if (string.IsNullOrWhiteSpace(spartanToken))
            {
                return LoginResult.Failed("Spartan Token 不能为空。");
            }

            try
            {
                var client = new HaloInfiniteClient(new StaticSpartanTokenProvider(spartanToken));
                var currentUser = await client.GetCurrentUserAsync().ConfigureAwait(false);
                await client.EnsureActiveFlightConfigurationAsync(currentUser.xuid).ConfigureAwait(false);
                var userInfo = await TryGetUserInfoAsync(client, currentUser.xuid).ConfigureAwait(false);

                var session = new SpartanAuthSession
                {
                    SpartanToken = spartanToken,
                    Xuid = currentUser.xuid,
                    Gamertag = userInfo?.Gamertag,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };

                await _sessionStore.SaveAsync(session).ConfigureAwait(false);
                return LoginResult.Success(session);
            }
            catch (Exception ex)
            {
                return LoginResult.Failed($"Token 验证失败：{ex.Message}");
            }
        }

        public async Task LogoutAsync()
        {
            await _sessionStore.ClearAsync().ConfigureAwait(false);
            await _cookieReader.ClearCookiesAsync().ConfigureAwait(false);
        }

        private static async Task<UserInfo> TryGetUserInfoAsync(HaloInfiniteClient client, string xuid)
        {
            if (string.IsNullOrWhiteSpace(xuid))
            {
                return null;
            }

            try
            {
                var users = await client.GetUsersAsync(new[] { xuid }).ConfigureAwait(false);
                return users?.FirstOrDefault();
            }
            catch
            {
                return null;
            }
        }
    }
}
