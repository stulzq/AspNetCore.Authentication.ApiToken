using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
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