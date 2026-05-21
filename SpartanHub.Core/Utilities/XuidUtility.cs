using System;

namespace SpartanHub.Core.Utilities
{
    public static class XuidUtility
    {
        public static string WrapPlayerId(string xuid)
        {
            return $"xuid({xuid})";
        }

        public static string UnwrapPlayerId(string xuid)
        {
            if (xuid.StartsWith("xuid(") && xuid.EndsWith(")"))
            {
                return xuid.Substring(5, xuid.Length - 6);
            }
            return xuid;
        }
    }
}
