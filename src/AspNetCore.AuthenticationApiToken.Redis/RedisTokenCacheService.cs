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
            _tokenCacheKeyPrefix = _options.CachePrefix + ":{0}:token:{1}";
        }

        public async Task InitializeAsync()
        {
            _connection = await ConnectionMultiplexer.ConnectAsync(_options.ConnectionString);
            _cache = _connection.GetDatabase();

            _logger.LogInformation("Redis token cache service init successful.");
        }

        private string GetKey(string token, string scheme)
        {
            return string.Format(_tokenCacheKeyPrefix, scheme, token);
        }

        public async Task<TokenModelCache> GetAsync(string token, string scheme)
        {
            var key = GetKey(token, scheme);
            var cacheData = await _cache.StringGetAsync(key);
            if (!cacheData.HasValue)
            {
                return default;
            }

            return MessagePackSerializer.Deserialize<TokenModelCache>(cacheData, ContractlessStandardResolver.Options);
        }

        public async Task SetAsync(TokenModel token)
        {
            var key = GetKey(token.Scheme, token.Value);

            await _cache.StringSetAsync(key, Serialize(new TokenModelCache() { Token = token }), token.LifeTime);
        }

        public async Task SetNullAsync(string invalidToken, string scheme)
        {
            var key = GetKey(scheme, invalidToken);
            if (_options.InvalidTokenNullCacheTimeSpan != null)
            {
                var ttl = _options.InvalidTokenNullCacheTimeSpan.Value;
                await _cache.StringSetAsync(key, Serialize(new TokenModelCache()), ttl);
            }
        }

        public async Task RemoveAsync(string token, string scheme)
        {
            var key = GetKey(scheme, token);

            await _cache.KeyDeleteAsync(key);
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

        private static byte[] Serialize(TokenModelCache data)
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
