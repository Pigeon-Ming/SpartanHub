using System;
using System.Threading.Tasks;
using SpartanHub.Core.Authentication;
using SpartanHub.Core.Clients;
using SpartanHub.Core.Models;
using Windows.Security.Credentials;

namespace SpartanHub.Service
{
    public class UserSessionService : ISpartanTokenProvider
    {
        private static readonly Lazy<UserSessionService> _instance = new Lazy<UserSessionService>(() => new UserSessionService());
        
        public static UserSessionService Instance => _instance.Value;

        private readonly HaloAuthenticationClient _authClient;
        private readonly HaloInfiniteClient _haloClient;
        
        private CachedUserToken _cachedToken;

        public string SpartanToken => _cachedToken?.Token;
        public UserInfo CurrentUser { get; private set; }
        public bool IsLoggedIn => !string.IsNullOrEmpty(SpartanToken);

        private UserSessionService()
        {
            _authClient = new HaloAuthenticationClient(new XboxAuthenticationClient(GetOauthTokenAsync));
            _haloClient = new HaloInfiniteClient(this);
            
            LoadTokenFromVault();
        }

        public async Task<string> GetSpartanTokenAsync()
        {
            if (_cachedToken == null || _cachedToken.IsExpired)
            {
                await RefreshTokenAsync();
            }
            return SpartanToken;
        }

        public async Task ClearSpartanTokenAsync()
        {
            await _authClient.ClearSpartanTokenAsync();
            _cachedToken = null;
        }

        public async Task<bool> LoginAsync(string oauthToken)
        {
            try
            {
                await _authClient.ClearSpartanTokenAsync();
                
                var spartanToken = await _authClient.GetSpartanTokenAsync();
                
                _cachedToken = new CachedUserToken
                {
                    Token = spartanToken,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };
                
                SaveTokenToVault(spartanToken);
                
                CurrentUser = await _haloClient.GetCurrentUserAsync();
                
                return true;
            }
            catch (Exception)
            {
                _cachedToken = null;
                CurrentUser = null;
                return false;
            }
        }

        public void Logout()
        {
            _cachedToken = null;
            CurrentUser = null;
            ClearTokenFromVault();
        }

        public async Task<bool> RefreshTokenAsync()
        {
            if (!IsLoggedIn)
            {
                return false;
            }

            try
            {
                await _authClient.ClearSpartanTokenAsync();
                var newToken = await _authClient.GetSpartanTokenAsync();
                
                _cachedToken = new CachedUserToken
                {
                    Token = newToken,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };
                
                SaveTokenToVault(newToken);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private async Task<string> GetOauthTokenAsync()
        {
            var vault = new PasswordVault();
            try
            {
                var credential = vault.Retrieve("SpartanHub_OAuth", "OAuthToken");
                return credential.Password;
            }
            catch
            {
                return null;
            }
        }

        private void SaveTokenToVault(string token)
        {
            try
            {
                var vault = new PasswordVault();
                vault.Add(new PasswordCredential("SpartanHub", "SpartanToken", token));
            }
            catch
            {
            }
        }

        private void LoadTokenFromVault()
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve("SpartanHub", "SpartanToken");
                _cachedToken = new CachedUserToken
                {
                    Token = credential.Password,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };
            }
            catch
            {
                _cachedToken = null;
            }
        }

        private void ClearTokenFromVault()
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve("SpartanHub", "SpartanToken");
                vault.Remove(credential);
            }
            catch
            {
            }
        }

        private class CachedUserToken
        {
            public string Token { get; set; }
            public DateTimeOffset ExpiresAt { get; set; }
            public bool IsExpired => DateTimeOffset.UtcNow > ExpiresAt;
        }
    }
}
