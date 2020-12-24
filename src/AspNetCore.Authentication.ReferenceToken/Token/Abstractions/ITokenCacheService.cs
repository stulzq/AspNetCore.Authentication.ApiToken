using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenCacheService
    {
        Task<TokenCacheModel> GetAsync(string token);

        Task SetAsync(TokenModel token);

        Task SetNullAsync(string invalidToken);

        Task RemoveAsync(TokenModel token,string reason=null);
    }
}