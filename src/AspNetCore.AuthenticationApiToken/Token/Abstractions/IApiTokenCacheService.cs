using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenCacheService
    {
        Task<ApiTokenCache> GetAsync(string token);

        Task SetAsync(ApiToken token);

        Task SetNullAsync(string invalidToken);

        Task RemoveAsync(ApiToken token,string reason=null);
    }
}