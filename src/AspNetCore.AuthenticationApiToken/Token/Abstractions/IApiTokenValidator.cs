using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string token, string schemeName);
    }
}