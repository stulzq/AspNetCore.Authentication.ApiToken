using System.Security.Claims;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public class DefaultTokenValidator : ITokenValidator
    {
        public virtual Task<ClaimsPrincipal> ValidateTokenAsync(string token)
        {
            throw new System.NotImplementedException();
        }
    }
}