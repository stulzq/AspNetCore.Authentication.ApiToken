using System.Security.Claims;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface IProfileService
    {
        Claim[] GetUserClaims();
    }
}