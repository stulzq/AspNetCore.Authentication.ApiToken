using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.ApiToken.SampleApp
{
    public class MyApiTokenStore : IApiTokenStore
    {
        public Task StoreAsync(ApiTokenModel token)
        {
            return Task.CompletedTask;
        }

        public Task StoreAsync(List<ApiTokenModel> token)
        {
            return Task.CompletedTask;
        }

        public Task<ApiTokenModel> GetAsync(string token)
        {
            return Task.FromResult(new ApiTokenModel()
            {
                Claims = new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ApiTokenClaimTypes.Subject, "1"),
                    new Claim(ApiTokenClaimTypes.Name,"张三"),
                },
                CreateTime = DateTimeOffset.Now,
                Expiration = DateTimeOffset.Now,
                Token = "A27145E8ED6DEE0451A9784454148F58FF9E96DFE3228B5331C7792BD6A91257",
                Type = ApiTokenType.ApiToken,
                UserId = "1"
            });
        }

        public Task<List<ApiTokenModel>> GetListAsync(string userId)
        {
            return Task.FromResult(new List<ApiTokenModel>()
            {
                new ApiTokenModel()
                {
                    Claims = new Claim[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ApiTokenClaimTypes.Subject, "1"),
                        new Claim(ApiTokenClaimTypes.Name,"张三"),
                    },
                    CreateTime = DateTimeOffset.Now,
                    Expiration = DateTimeOffset.Now,
                    Token = "A27145E8ED6DEE0451A9784454148F58FF9E96DFE3228B5331C7792BD6A91257",
                    Type = ApiTokenType.ApiToken,
                    UserId = "1"
                }
            });
        }

        public Task<ApiTokenModel> UpdateClaimsAsync(string token, Claim[] claims)
        {
            throw new NotImplementedException();
        }

        public Task<ApiTokenModel> RemoveAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task<ApiTokenModel> RemoveListAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task RemoveExpirationAsync()
        {
            throw new NotImplementedException();
        }
    }
}