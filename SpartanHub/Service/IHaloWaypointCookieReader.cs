using System.Threading.Tasks;

namespace SpartanHub.Service
{
    public interface IHaloWaypointCookieReader
    {
        Task<string> TryGetSpartanTokenAsync();
        Task ClearCookiesAsync();
    }
}
