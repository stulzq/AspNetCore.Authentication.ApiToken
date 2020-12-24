using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class NullTokenCacheService:ITokenCacheService
    {
        public Task<TokenModel> GetAsync(string token)
        {
            return Task.FromResult(default(TokenModel));
        }

        public Task SetAsync(TokenModel token)
        {
            return Task.CompletedTask;
        }
    }
}