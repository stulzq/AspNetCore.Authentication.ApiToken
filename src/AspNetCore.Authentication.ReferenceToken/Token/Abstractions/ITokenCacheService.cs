using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenCacheService
    {
        Task<TokenModel> GetAsync(string token);

        Task SetAsync(TokenModel token);

        Task RemoveAsync(TokenModel token,string reason=null);
    }
}