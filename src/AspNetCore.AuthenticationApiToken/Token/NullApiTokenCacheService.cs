using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.Authentication.ApiToken
{
    public class NullApiTokenCacheService:IApiTokenCacheService
    {
        public Task<ApiTokenCache> GetAsync(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task SetAsync(ApiTokenModel token)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNullAsync(string invalidToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(ApiTokenModel token, string reason = null)
        {
            throw new System.NotImplementedException();
        }
    }
}