namespace SpartanHub.Core
{
    public static class GlobalConstants
    {
        public const string HaloWaypointUserAgent = "HaloWaypoint/2021112313511900 CFNetwork/1327.0.4 Darwin/21.2.0";
        public const string HaloPcUserAgent = "SHIVA-2043073184/6.10021.18539.0 (release; PC)";
        public static string[] DefaultAuthScopes { get; } = { "Xboxlive.signin", "Xboxlive.offline_access" };
    }
}
