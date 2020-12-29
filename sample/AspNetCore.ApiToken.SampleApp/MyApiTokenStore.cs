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
        public Task StoreAsync(TokenModel token)
        {
            return Task.CompletedTask;
        }

        public Task StoreAsync(List<TokenModel> token)
        {
            return Task.CompletedTask;
        }

        public Task<TokenModel> GetAsync(string token,string scheme)
        {
            if (token != "A27145E8ED6DEE0451A9784454148F58FF9E96DFE3228B5331C7792BD6A91257")
            {
                return Task.FromResult(default(TokenModel));
            }
            return Task.FromResult(new TokenModel()
            {
                Claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.NameIdentifier, "1"),
                    new Claim(ApiTokenClaimTypes.Subject, "1"),
                    new Claim(ApiTokenClaimTypes.Name,"张三"),
                    new Claim(ApiTokenClaimTypes.Role,"Admin"),
                },
                CreateTime = DateTimeOffset.Now,
                Expiration = DateTimeOffset.Now.AddHours(1),
                Value = "A27145E8ED6DEE0451A9784454148F58FF9E96DFE3228B5331C7792BD6A91257",
                Type = TokenType.Bearer,
                UserId = "1"
            });
        }

        public Task<List<TokenModel>> GetListAsync(string userId,string scheme)
        {
            return Task.FromResult(new List<TokenModel>()
            {
                new TokenModel()
                {
                    Claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, "1"),
                        new Claim(ApiTokenClaimTypes.Subject, "1"),
                        new Claim(ApiTokenClaimTypes.Name,"张三"),
                        new Claim(ApiTokenClaimTypes.Role,"Admin"),
                    },
                    CreateTime = DateTimeOffset.Now,
                    Expiration = DateTimeOffset.Now,
                    Value = "A27145E8ED6DEE0451A9784454148F58FF9E96DFE3228B5331C7792BD6A91257",
                    Type = TokenType.Bearer,
                    UserId = "1"
                }
            });
        }

        public Task<List<TokenModel>> GetListAsync(string userId, string scheme, TokenType type)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TokenModel token)
        {
            throw new NotImplementedException();
        }

        public Task UpdateListAsync(List<TokenModel> token)
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync(TokenModel token,string scheme)
        {
            throw new NotImplementedException();
        }


        public Task RemoveAsync(string token, string scheme)
        {
            throw new NotImplementedException();
        }

        public Task RemoveListAsync(string userId, string scheme)
        {
            throw new NotImplementedException();
        }

        public Task RemoveListAsync(string userId, string scheme, TokenType type)
        {
            throw new NotImplementedException();
        }

        public Task<int> RemoveExpirationAsync()
        {
            return Task.FromResult(0);
        }
    }
}