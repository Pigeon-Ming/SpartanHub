using System.Threading.Tasks;

namespace SpartanHub.Core.Authentication
{
    public interface IPersistedSpartanSessionStore
    {
        Task<SpartanAuthSession> LoadAsync();
        Task SaveAsync(SpartanAuthSession session);
        Task ClearAsync();
    }
}
