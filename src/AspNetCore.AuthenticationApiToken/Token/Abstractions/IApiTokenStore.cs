using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenStore
    {
        Task StoreAsync(ApiToken token);
        Task StoreAsync(List<ApiToken> token);

        Task<ApiToken> GetAsync(string token);

        Task<List<ApiToken>> GetListAsync(string userId);

        Task<ApiToken> UpdateClaimsAsync(string token,Claim[] claims);

        Task<ApiToken> RemoveAsync(string token);

        Task<ApiToken> RemoveListAsync(string userId);

        Task RemoveExpirationAsync();
    }
}