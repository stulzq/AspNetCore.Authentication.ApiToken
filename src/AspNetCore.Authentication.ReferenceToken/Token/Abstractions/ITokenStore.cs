using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenStore
    {
        Task StoreAsync(TokenModel token);
        Task StoreAsync(List<TokenModel> token);

        Task<TokenModel> GetAsync(string token);

        Task<List<TokenModel>> GetListAsync(string userId);

        Task<TokenModel> UpdateClaimsAsync(string token,Claim[] claims);

        Task<TokenModel> RemoveAsync(string token, string reason);

        Task<TokenModel> RemoveListAsync(string userId, string reason);

        Task RemoveExpirationAsync();
    }
}