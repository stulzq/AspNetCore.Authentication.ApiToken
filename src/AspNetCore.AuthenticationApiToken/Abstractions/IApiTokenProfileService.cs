using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenProfileService
    {
        Task<List<Claim>> GetUserClaimsAsync(string userId);
    }
}