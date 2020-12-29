using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.Authentication.ApiToken
{
    public class NullApiTokenCacheService : IApiTokenCacheService
    {
        public Task<TokenModelCache> GetAsync(string token, string scheme)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(TokenModel token)
        {
            return Task.CompletedTask;
        }

        public Task SetNullAsync(string invalidToken, string scheme)
        {
            return Task.CompletedTask;
        }

        public Task RemoveAsync(string token, string scheme)
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