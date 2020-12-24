using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenValidator
    {
        Task<ClaimsPrincipal> ValidateTokenAsync(string token);
    }
}