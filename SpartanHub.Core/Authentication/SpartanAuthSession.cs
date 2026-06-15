using System;

namespace SpartanHub.Core.Authentication
{
    public class SpartanAuthSession
    {
        public string SpartanToken { get; set; }
        public DateTimeOffset? ExpiresAt { get; set; }
        public string Xuid { get; set; }
        public string Gamertag { get; set; }

        public bool HasToken => !string.IsNullOrWhiteSpace(SpartanToken);

        public bool IsExpired =>
            ExpiresAt.HasValue && ExpiresAt.Value <= DateTimeOffset.UtcNow.AddMinutes(5);
    }
}
