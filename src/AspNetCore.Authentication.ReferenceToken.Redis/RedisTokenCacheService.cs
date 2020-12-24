using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AspNetCore.Authentication.ReferenceToken.Redis
{
    public class RedisTokenCacheService : ITokenCacheService
    {
        private readonly ReferenceTokenOptions _tokenOptions;
        private readonly RedisTokenCacheOptions _options;

        private readonly IDatabase _cache;

        private readonly string _tokenCacheKeyPrefix;
        public RedisTokenCacheService(IOptions<RedisTokenCacheOptions> options, 
            IOptions<ReferenceTokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
            _options = options.Value;

            var connection = ConnectionMultiplexer.Connect(_options.ConnectionString);
            _cache = connection.GetDatabase();

            _tokenCacheKeyPrefix = _options.CachePrefix + "token:{0}";
        }


        public async Task<TokenCacheModel> GetAsync(string token)
        {
            var cacheData = await _cache.StringGetAsync(_tokenCacheKeyPrefix);
            if (!cacheData.HasValue)
            {
                return default;
            }

            return MessagePackSerializer.Deserialize<TokenCacheModel>(cacheData, ContractlessStandardResolver.Options);
        }

        public async Task SetAsync(TokenModel token)
        {
            TimeSpan ttl;
            if (token.IsExpired(_tokenOptions.TokenExpireClockSkew))
            {
                ttl = _tokenOptions.TokenExpireClockSkew;
            }
            else
            {
                ttl= token.GetLifeTime(_tokenOptions.TokenExpireClockSkew);
            }

            await _cache.StringSetAsync(_tokenCacheKeyPrefix, Serialize(new TokenCacheModel() { Token = token }), ttl);
        }

        public async Task SetNullAsync(string invalidToken)
        {
            if (_options.PreventPenetration != null)
            {
                var cacheData = new TokenCacheModel();
                var ttl = _options.PreventPenetration.Value;

                await _cache.StringSetAsync(_tokenCacheKeyPrefix, Serialize(cacheData), ttl);
            }
        }

        public async Task RemoveAsync(TokenModel token, string reason = null)
        {
            var key = string.Format(_tokenCacheKeyPrefix, token.Token);
            if (string.IsNullOrEmpty(reason))
            {
                await _cache.KeyDeleteAsync(key);
            }
            else
            {
                var ttl = _tokenOptions.TokenExpireClockSkew;
                await _cache.KeyExpireAsync(key,ttl);
            }
        }

        private byte[] Serialize(TokenCacheModel data)
        {
            return MessagePackSerializer.Serialize(data, ContractlessStandardResolver.Options);
        }

    }
}
