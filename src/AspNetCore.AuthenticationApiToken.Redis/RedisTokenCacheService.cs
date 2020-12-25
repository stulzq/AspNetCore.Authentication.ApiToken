using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using MessagePack;
using MessagePack.Resolvers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace AspNetCore.Authentication.ApiToken.Redis
{
    public class RedisTokenCacheService : IApiTokenCacheService, IAsyncDisposable
    {
        private readonly ILogger<RedisTokenCacheService> _logger;
        private readonly RedisTokenCacheOptions _options;
        private IDatabase _cache;
        private ConnectionMultiplexer _connection;
        private readonly string _tokenCacheKeyPrefix;
        public RedisTokenCacheService(IOptions<RedisTokenCacheOptions> options, ILogger<RedisTokenCacheService> logger)
        {
            _logger = logger;
            _options = options.Value;
            _tokenCacheKeyPrefix = _options.CachePrefix + ":token:{0}";
        }

        public async Task InitializeAsync()
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConnectionString);
            _cache = _connection.GetDatabase();

            _logger.LogInformation("Redis token cache service init successful.");
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
                await _cache.StringSetAsync(key, Serialize(new ApiTokenCache() { Reason = reason }),
                    _options.InvalidTokenReasonCacheTTL.Value);
            }
            else
            {

                await _cache.KeyDeleteAsync(key);
            }
        }

        public async Task<bool> LockTakeAsync(string key, string value, TimeSpan timeOut)
        {
            var lockKey = string.Format(_tokenCacheKeyPrefix, key);
            return await _cache.LockTakeAsync(lockKey, value, timeOut);
        }

        public async Task LockReleaseAsync(string key, string value)
        {
            var lockKey = string.Format(_tokenCacheKeyPrefix, key);
            var result = await _cache.LockReleaseAsync(lockKey, value);
            if (!result)
            {
                _logger.LogError($"Lock release failed, Key: {lockKey}, Value: {value}");
            }
        }

        private static byte[] Serialize(ApiTokenCache data)
        {
            return MessagePackSerializer.Serialize(data, ContractlessStandardResolver.Options);
        }

        public async ValueTask DisposeAsync()
        {
            if (_connection != null)
            {
                await _connection.CloseAsync();
            }
        }
    }
}
