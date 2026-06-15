using System;
using System.Threading.Tasks;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Models;

namespace SpartanHub.Service
{
    public class UserSessionService : ISpartanTokenProvider, IPlayerContextProvider
    {
        private static readonly Lazy<UserSessionService> _instance = new Lazy<UserSessionService>(() => new UserSessionService());
        
        public static UserSessionService Instance => _instance.Value;

        private readonly IPersistedSpartanSessionStore _sessionStore;
        private readonly IHaloWaypointLoginService _loginService;
        
        private SpartanAuthSession _session;

        public string SpartanToken => _session?.SpartanToken;
        public string PlayerXuid => _session?.Xuid;
        public UserInfo CurrentUser { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(SpartanToken);

        private UserSessionService()
        {
            _sessionStore = new PasswordVaultSpartanSessionStore();
            _loginService = new HaloWaypointLoginService(new UwpWebViewCookieReader(), _sessionStore);
            
            LoadSessionFromVault();
        }

        public async Task<string> GetSpartanTokenAsync()
        {
            if (_session == null || !_session.HasToken)
            {
                throw new InvalidOperationException("当前没有可用的 Spartan Token，请先登录 HaloWaypoint。");
            }

            if (_session.IsExpired)
            {
                var refreshed = await RefreshTokenAsync().ConfigureAwait(false);
                if (!refreshed)
                {
                    throw new InvalidOperationException("Spartan Token 已过期，请重新登录 HaloWaypoint。");
                }
            }

            return SpartanToken;
        }

        public async Task ClearSpartanTokenAsync()
        {
            await LogoutAsync().ConfigureAwait(false);
        }

        public async Task<bool> LoginAsync(string spartanToken)
        {
            return await SignInWithSpartanTokenAsync(spartanToken).ConfigureAwait(false);
        }

        public async Task<bool> SignInWithSpartanTokenAsync(string spartanToken)
        {
            var result = await _loginService.SignInWithSpartanTokenAsync(spartanToken).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                ClearInMemorySession();
                return false;
            }

            ApplySession(result.Session);
            return true;
        }

        public async Task<LoginResult> CompleteHaloWaypointLoginAsync()
        {
            var result = await _loginService.CompleteLoginAsync().ConfigureAwait(false);
            if (result.IsSuccess)
            {
                ApplySession(result.Session);
            }

            return result;
        }

        public void Logout()
        {
            ClearInMemorySession();
            _ = _loginService.LogoutAsync();
        }

        public async Task LogoutAsync()
        {
            ClearInMemorySession();
            await _loginService.LogoutAsync().ConfigureAwait(false);
        }

        public async Task<bool> RefreshTokenAsync()
        {
            if (!IsLoggedIn)
            {
                return false;
            }

            try
            {
                var result = await _loginService.SignInWithSpartanTokenAsync(SpartanToken).ConfigureAwait(false);
                if (!result.IsSuccess)
                {
                    return false;
                }

                ApplySession(result.Session);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void LoadSessionFromVault()
        {
            try
            {
                var session = _sessionStore.LoadAsync().GetAwaiter().GetResult();
                if (session != null && session.HasToken && !session.IsExpired)
                {
                    ApplySession(session);
                }
            }
            catch
            {
                ClearInMemorySession();
            }
        }

        private void ApplySession(SpartanAuthSession session)
        {
            _session = session;
            CurrentUser = session == null
                ? null
                : new UserInfo
                {
                    Xuid = session.Xuid,
                    Gamertag = session.Gamertag,
                    Gamerpic = null
                };
        }

        private void ClearInMemorySession()
        {
            _session = null;
            CurrentUser = null;
        }

        public async Task<bool> LoadPersistedSessionAsync()
        {
            var session = await _sessionStore.LoadAsync().ConfigureAwait(false);
            if (session == null || !session.HasToken || session.IsExpired)
            {
                ClearInMemorySession();
                return false;
            }

            ApplySession(session);
            return true;
        }
    }
}
