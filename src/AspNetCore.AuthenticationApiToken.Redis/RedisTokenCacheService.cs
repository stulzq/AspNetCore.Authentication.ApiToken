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
        private readonly RedisTokenCacheOptions _options;
        private IDatabase _cache;
        private readonly string _tokenCacheKeyPrefix;
        public RedisTokenCacheService(IOptions<RedisTokenCacheOptions> options)
        {
            _options = options.Value;
            _tokenCacheKeyPrefix = _options.CachePrefix + ":token:{0}";
        }

        public async Task InitializeAsync()
        {
            var connection = await ConnectionMultiplexer.ConnectAsync(_options.ConnectionString);
            _cache = connection.GetDatabase();
        }

        public async Task<ApiTokenCache> GetAsync(string token)
        {
            var key = string.Format(_tokenCacheKeyPrefix, token);
            var cacheData = await _cache.StringGetAsync(key);
            if (!cacheData.HasValue)
            {
                return default;
            }

            return MessagePackSerializer.Deserialize<ApiTokenCache>(cacheData, ContractlessStandardResolver.Options);
        }

        public async Task SetAsync(TokenModel token)
        {
            var key = string.Format(_tokenCacheKeyPrefix, token.Value);
            if (!token.IsValid)
            {
                return;
            }

            await _cache.StringSetAsync(key, Serialize(new ApiTokenCache() { Token = token }), token.LifeTime);
        }

        public async Task SetNullAsync(string invalidToken)
        {
            var key = string.Format(_tokenCacheKeyPrefix, invalidToken);
            if (_options.InvalidTokenNullCacheTTL != null)
            {
                var nullCache = new ApiTokenCache();
                var ttl = _options.InvalidTokenNullCacheTTL.Value;
                await _cache.StringSetAsync(key, Serialize(nullCache), ttl);
            }
        }

        public async Task RemoveAsync(string token, string reason = null)
        {
            var key = string.Format(_tokenCacheKeyPrefix, token);

            if (_options.InvalidTokenReasonCacheTTL != null && !string.IsNullOrEmpty(reason))
            {
                await _cache.StringSetAsync(key, Serialize(new ApiTokenCache() {Reason = reason}),
                    _options.InvalidTokenReasonCacheTTL.Value);
            }
            else
            {

                await _cache.KeyDeleteAsync(key);
            }
        }

        private static byte[] Serialize(ApiTokenCache data)
        {
            return MessagePackSerializer.Serialize(data, ContractlessStandardResolver.Options);
        }

    }
}
