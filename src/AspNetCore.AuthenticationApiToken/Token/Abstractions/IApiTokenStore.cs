using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenStore
    {
        Task StoreAsync(ApiTokenModel token);
        Task StoreAsync(List<ApiTokenModel> token);

        Task<ApiTokenModel> GetAsync(string token);

        Task<List<ApiTokenModel>> GetListAsync(string userId);

        Task<ApiTokenModel> UpdateClaimsAsync(string token,IReadOnlyList<Claim> claims);

        Task<ApiTokenModel> RemoveAsync(string token);

        Task<ApiTokenModel> RemoveListAsync(string userId);

        Task RemoveExpirationAsync();
    }
}