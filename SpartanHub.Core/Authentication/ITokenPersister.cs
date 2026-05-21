using System.Threading.Tasks;

namespace SpartanHub.Core.Authentication
{
    public interface ITokenPersister
    {
        Task<string> LoadAsync(string key);
        Task SaveAsync(string key, string value);
        Task ClearAsync(string key);
    }
}
