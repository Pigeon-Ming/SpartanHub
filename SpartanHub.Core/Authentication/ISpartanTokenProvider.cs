using System.Threading.Tasks;

namespace SpartanHub.Core.Authentication
{
    public interface ISpartanTokenProvider
    {
        Task<string> GetSpartanTokenAsync();
        Task ClearSpartanTokenAsync();
    }
}
