using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.Authentication.ApiToken
{
    public class NullApiTokenCacheService : IApiTokenCacheService
    {
        public Task<ApiTokenCache> GetAsync(string token)
        {
            return Task.FromResult(default(ApiTokenCache));
        }

        public Task SetAsync(TokenModel token)
        {
            return Task.CompletedTask;
        }

        public Task SetNullAsync(string invalidToken)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string token, string reason = null)
        {
            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}