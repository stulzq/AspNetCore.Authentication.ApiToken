using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenStore
    {
        Task StoreAsync(TokenModel token);
        Task StoreAsync(List<TokenModel> token);

        Task<TokenModel> GetAsync(string token);

        Task<List<TokenModel>> GetListAsync(string userId);

        Task<TokenModel> UpdateClaimsAsync(string token,IReadOnlyList<Claim> claims);

        Task<TokenModel> RemoveAsync(string token);

        Task<TokenModel> RemoveListAsync(string userId);

        Task<int> RemoveExpirationAsync();
    }
}