using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SpartanHub.Core.Authentication;
using Windows.Security.Credentials;

namespace SpartanHub.Service
{
    public class PasswordVaultSpartanSessionStore : IPersistedSpartanSessionStore
    {
        private const string ResourceName = "SpartanHub.AuthSession";
        private const string UserName = "CurrentUser";

        public Task<SpartanAuthSession> LoadAsync()
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve(ResourceName, UserName);
                var json = credential.Password;

                if (string.IsNullOrWhiteSpace(json))
                {
                    return Task.FromResult(TryLoadLegacyToken());
                }

                return Task.FromResult(JsonConvert.DeserializeObject<SpartanAuthSession>(json));
            }
            catch
            {
                return Task.FromResult(TryLoadLegacyToken());
            }
        }

        public async Task SaveAsync(SpartanAuthSession session)
        {
            if (session == null || string.IsNullOrWhiteSpace(session.SpartanToken))
            {
                throw new ArgumentException("Session must contain a Spartan token.", nameof(session));
            }

            await ClearAsync().ConfigureAwait(false);

            var json = JsonConvert.SerializeObject(session);
            var vault = new PasswordVault();
            vault.Add(new PasswordCredential(ResourceName, UserName, json));
        }

        public Task ClearAsync()
        {
            try
            {
                var vault = new PasswordVault();
                var credentials = vault.FindAllByResource(ResourceName).ToArray();

                foreach (var credential in credentials)
                {
                    vault.Remove(credential);
                }
            }
            catch
            {
            }

            ClearLegacyToken();
            return Task.CompletedTask;
        }

        private static void ClearLegacyToken()
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

        private static SpartanAuthSession TryLoadLegacyToken()
        {
            try
            {
                var vault = new PasswordVault();
                var credential = vault.Retrieve("SpartanHub", "SpartanToken");
                if (string.IsNullOrWhiteSpace(credential.Password))
                {
                    return null;
                }

                return new SpartanAuthSession
                {
                    SpartanToken = credential.Password,
                    ExpiresAt = DateTimeOffset.UtcNow.AddHours(24)
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
