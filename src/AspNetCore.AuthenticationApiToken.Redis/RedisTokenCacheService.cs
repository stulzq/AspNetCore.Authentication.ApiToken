using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AspNetCore.Authentication.ApiToken.Redis
{
    public class RedisTokenCacheService : IApiTokenCacheService
    {
        private readonly ApiTokenOptions _tokenOptions;
        private readonly RedisTokenCacheOptions _options;

        private readonly IDatabase _cache;

        private readonly string _tokenCacheKeyPrefix;
        public RedisTokenCacheService(IOptions<RedisTokenCacheOptions> options, 
            IOptions<ApiTokenOptions> tokenOptions)
        {
            _tokenOptions = tokenOptions.Value;
            _options = options.Value;

            var connection = ConnectionMultiplexer.Connect(_options.ConnectionString);
            _cache = connection.GetDatabase();

            _tokenCacheKeyPrefix = _options.CachePrefix + "token:{0}";
        }


        public async Task<ApiTokenCache> GetAsync(string token)
        {
            var cacheData = await _cache.StringGetAsync(_tokenCacheKeyPrefix);
            if (!cacheData.HasValue)
            {
                return default;
            }

            return MessagePackSerializer.Deserialize<ApiTokenCache>(cacheData, ContractlessStandardResolver.Options);
        }

        public async Task SetAsync(ApiToken token)
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

            await _cache.StringSetAsync(_tokenCacheKeyPrefix, Serialize(new ApiTokenCache() { Token = token }), ttl);
        }

        public async Task SetNullAsync(string invalidToken)
        {
            if (_options.PreventPenetration != null)
            {
                var cacheData = new ApiTokenCache();
                var ttl = _options.PreventPenetration.Value;

                await _cache.StringSetAsync(_tokenCacheKeyPrefix, Serialize(cacheData), ttl);
            }
        }

        public async Task RemoveAsync(ApiToken token, string reason = null)
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

        private static byte[] Serialize(ApiTokenCache data)
        {
            return MessagePackSerializer.Serialize(data, ContractlessStandardResolver.Options);
        }

    }
}
