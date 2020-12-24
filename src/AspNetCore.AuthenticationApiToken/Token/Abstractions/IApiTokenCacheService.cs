using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenCacheService
    {
        Task<ApiTokenCache> GetAsync(string token);

        Task SetAsync(ApiTokenModel token);

        Task SetNullAsync(string invalidToken);

        Task RemoveAsync(ApiTokenModel token,string reason=null);
    }
}