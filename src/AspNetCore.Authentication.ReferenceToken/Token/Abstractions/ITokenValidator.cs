using System.Security.Claims;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenValidator
    {
        ClaimsPrincipal ValidateToken(string token);
    }
}