using System;
using System.Threading.Tasks;
using SpartanHub.Core.Authentication;

namespace SpartanHub.Core.Authentication
{
    public class StaticSpartanTokenProvider : ISpartanTokenProvider
    {
        private string _token;

        public StaticSpartanTokenProvider(string token)
        {
            _token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public Task<string> GetSpartanTokenAsync()
        {
            return Task.FromResult(_token);
        }

        public Task ClearSpartanTokenAsync()
        {
            _token = null;
            return Task.CompletedTask;
        }

        public void SetToken(string token)
        {
            _token = token;
        }
    }
}
