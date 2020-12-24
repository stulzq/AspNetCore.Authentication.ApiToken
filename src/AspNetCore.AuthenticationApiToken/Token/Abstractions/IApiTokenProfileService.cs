using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenProfileService
    {
        Task<Claim[]> GetUserClaimsAsync(string userId);
    }
}