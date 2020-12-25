using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenCacheService
    {
        Task<ApiTokenCache> GetAsync(string token);

        Task SetAsync(TokenModel token);

        Task SetNullAsync(string invalidToken);

        Task RemoveAsync(string token,string reason=null);
        Task InitializeAsync();
    }
}