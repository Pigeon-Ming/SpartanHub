using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpartanHub.Core.Authentication
{
    public class InMemoryTokenPersister : ITokenPersister
    {
        private readonly Dictionary<string, string> _store = new Dictionary<string, string>();

        public Task<string> LoadAsync(string key)
        {
            if (_store.TryGetValue(key, out var value))
            {
                return Task.FromResult(value);
            }
            return Task.FromResult<string>(null);
        }

        public Task SaveAsync(string key, string value)
        {
            _store[key] = value;
            return Task.CompletedTask;
        }

        public Task ClearAsync(string key)
        {
            _store.Remove(key);
            return Task.CompletedTask;
        }
    }
}
