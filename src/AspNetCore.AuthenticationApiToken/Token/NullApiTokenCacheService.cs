using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.Authentication.ApiToken
{
    public class NullApiTokenCacheService : IApiTokenCacheService
    {
        public Task<ApiTokenCache> GetAsync(string token)
        {
            throw new NotImplementedException();
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

        public Task<bool> LockTakeAsync(string key, string value, TimeSpan timeOut)
        {
            return Task.FromResult(true);
        }

        public Task LockReleaseAsync(string key, string value)
        {
            return Task.FromResult(true);
        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
    }
}