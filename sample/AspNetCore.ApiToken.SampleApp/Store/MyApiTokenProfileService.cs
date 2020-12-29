using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.ApiToken.SampleApp.Store
{
    public class MyApiTokenProfileService : IApiTokenProfileService
    {
        private readonly ApiTokenDbContext _dbContext;

        public MyApiTokenProfileService(ApiTokenDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Claim>> GetUserClaimsAsync(string userId)
        {
            var realUserId = int.Parse(userId);
            var user = await _dbContext.Users.FirstAsync(a => a.Id == realUserId);
            return new List<Claim>()
            {
                new Claim(ApiTokenClaimTypes.Subject,userId),
                new Claim(ApiTokenClaimTypes.Name,user.Name),
                new Claim(ApiTokenClaimTypes.Role,user.Role),
            };
        }
    }
}