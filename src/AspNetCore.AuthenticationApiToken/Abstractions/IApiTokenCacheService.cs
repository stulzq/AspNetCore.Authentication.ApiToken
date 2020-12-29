using System;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenCacheService
    {
        Task InitializeAsync();

        Task<TokenModelCache> GetAsync(string token, string scheme);

        Task SetAsync(TokenModel token);

        Task SetNullAsync(string invalidToken, string scheme);

        Task RemoveAsync(string token, string scheme);

        Task<bool> LockTakeAsync(string key, string value, TimeSpan timeOut);

        Task LockReleaseAsync(string key, string value);
    }
}