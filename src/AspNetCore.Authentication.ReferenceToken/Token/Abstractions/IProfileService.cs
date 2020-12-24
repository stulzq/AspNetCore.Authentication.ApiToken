using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface IProfileService
    {
        Task<Claim[]> GetUserClaimsAsync(string userId);
    }
}