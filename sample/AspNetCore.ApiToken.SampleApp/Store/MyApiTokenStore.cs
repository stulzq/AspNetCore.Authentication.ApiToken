using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace AspNetCore.ApiToken.SampleApp.Store
{
    public class MyApiTokenStore : IApiTokenStore
    {
        private readonly ApiTokenDbContext _dbContext;

        public MyApiTokenStore(ApiTokenDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task StoreAsync(TokenModel token)
        {
            var entity = ConvertToApiToken(token);
            await _dbContext.Token.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task StoreAsync(List<TokenModel> token)
        {
            var entities = token.Select(ConvertToApiToken).ToList();
            await _dbContext.Token.AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<TokenModel> GetAsync(string token, string scheme)
        {
            var entity = await _dbContext.Token.Where(a => a.Token == token && a.Scheme == scheme).FirstOrDefaultAsync();
            return entity == null ? null : ConvertToTokenModel(entity);
        }

        public async Task<List<TokenModel>> GetListAsync(string userId, string scheme)
        {
            var queryResult = await _dbContext.Token.Where(a => a.UserId == int.Parse(userId) && a.Scheme == scheme).ToListAsync();
            var result = queryResult.Select(ConvertToTokenModel).ToList();
            return result;
        }

        public async Task<List<TokenModel>> GetListAsync(string userId, string scheme, TokenType type)
        {
            var queryResult = await _dbContext.Token
                .Where(a => a.UserId == int.Parse(userId) && a.Scheme == scheme)
                .Where(a => a.Token == type.ToString())
                .ToListAsync();
            var result = queryResult.Select(ConvertToTokenModel).ToList();
            return result;
        }

        public async Task UpdateAsync(TokenModel token)
        {
            _dbContext.Token.Update(ConvertToApiToken(token));
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateListAsync(List<TokenModel> token)
        {
            _dbContext.Token.UpdateRange(token.Select(ConvertToApiToken));
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveAsync(string token, string scheme)
        {
            var tokenEntity = await _dbContext.Token.Where(a => a.Token == token).FirstOrDefaultAsync();
            if (tokenEntity != null)
            {
                _dbContext.Token.Remove(tokenEntity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveListAsync(string userId, string scheme)
        {
            var tokenList = await _dbContext.Token
                .Where(a => a.UserId == int.Parse(userId) && a.Scheme == scheme)
                .ToListAsync();
            if (tokenList.Any())
            {
                _dbContext.Token.RemoveRange(tokenList);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task RemoveListAsync(string userId, string scheme, TokenType type)
        {
            var tokenList = await _dbContext.Token
                .Where(a => a.UserId == int.Parse(userId) && a.Scheme == scheme)
                .Where(a => a.Type == type.ToString())
                .ToListAsync();
            if (tokenList.Any())
            {
                _dbContext.Token.RemoveRange(tokenList);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<int> RemoveExpirationAsync()
        {
            var tokens = _dbContext.Token.Where(a => a.Expiration < DateTime.Now);
            var count = await tokens.CountAsync();
            _dbContext.Token.RemoveRange(tokens);
            await _dbContext.SaveChangesAsync();
            return count;
        }

        private TokenModel ConvertToTokenModel(Entities.ApiToken apiToken)
        {
            var result = new TokenModel()
            {
                CreateTime = apiToken.CreateTime,
                Expiration = apiToken.Expiration,
                Type = Enum.Parse<TokenType>(apiToken.Type),
                UserId = apiToken.UserId.ToString(),
                Value = apiToken.Token,
                Scheme = apiToken.Scheme,
                Claims = JsonConvert.DeserializeObject<List<Claim>>(apiToken.Claims,new ClaimConverter())
            };

            return result;

        }

        private Entities.ApiToken ConvertToApiToken(TokenModel tokenModel)
        {
            var result = new Entities.ApiToken()
            {
                CreateTime = tokenModel.CreateTime.DateTime,
                Expiration = tokenModel.Expiration.DateTime,
                Type = tokenModel.Type.ToString(),
                UserId = int.Parse(tokenModel.UserId),
                Token = tokenModel.Value,
                Scheme = tokenModel.Scheme
            };

            if (tokenModel.Claims != null)
            {
                result.Claims = JsonConvert.SerializeObject(tokenModel.Claims, new ClaimConverter());
            }
            else
            {
                result.Claims = "[]";
            }

            return result;
        }
    }
}