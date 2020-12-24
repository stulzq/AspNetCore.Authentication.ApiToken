using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.ApiToken.SampleApp
{
    public class MyApiTokenProfileService : IApiTokenProfileService
    {
        public Task<Claim[]> GetUserClaimsAsync(string userId)
        {
            return Task.FromResult(new Claim[] {new Claim(ApiTokenClaimTypes.Subject, userId)});
        }
    }
}