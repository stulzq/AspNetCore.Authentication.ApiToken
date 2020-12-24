using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.ApiToken.SampleApp
{
    public class MyApiTokenProfileService : IApiTokenProfileService
    {
        public Task<List<Claim>> GetUserClaimsAsync(string userId)
        {
            return Task.FromResult(new List<Claim>() {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ApiTokenClaimTypes.Subject, "1"),
                new Claim(ApiTokenClaimTypes.Name,"张三"),
                new Claim(ApiTokenClaimTypes.Role,"Admin"),

            }
            );
        }
    }
}