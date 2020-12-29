using System;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenCacheService
    {
        Task InitializeAsync();

        Task<TokenModelCache> GetAsync(string token);

        Task SetAsync(TokenModel token);

        Task SetNullAsync(string invalidToken);

        Task RemoveAsync(string token);

        Task<bool> LockTakeAsync(string key, string value, TimeSpan timeOut);

        Task LockReleaseAsync(string key, string value);
    }
}