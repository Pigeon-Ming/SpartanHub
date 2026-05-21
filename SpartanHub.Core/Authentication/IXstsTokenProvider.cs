using System.Threading.Tasks;

namespace SpartanHub.Core.Authentication
{
    public interface IXstsTokenProvider
    {
        Task<string> FetchTokenAsync();
        Task ClearXstsTokenAsync();
    }
}
