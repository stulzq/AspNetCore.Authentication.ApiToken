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

        public Task SetAsync(ApiToken token)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNullAsync(string invalidToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(ApiToken token, string reason = null)
        {
            throw new System.NotImplementedException();
        }
    }
}