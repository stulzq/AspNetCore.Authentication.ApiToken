using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class NullTokenCacheService:ITokenCacheService
    {
        public Task<TokenCacheModel> GetAsync(string token)
        {
            throw new System.NotImplementedException();
        }

        public Task SetAsync(TokenModel token)
        {
            throw new System.NotImplementedException();
        }

        public Task SetNullAsync(string invalidToken)
        {
            throw new System.NotImplementedException();
        }

        public Task RemoveAsync(TokenModel token, string reason = null)
        {
            throw new System.NotImplementedException();
        }
    }
}