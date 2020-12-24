using System;

namespace AspNetCore.Authentication.ReferenceToken.Redis
{
    public class RedisTokenCacheOptions
    {
        public string ConnectionString { get; set; }

        public TimeSpan? PreventPenetration { get; set; } = TimeSpan.FromHours(10);

        /// <summary>
        /// Use the bloom filter to determine whether the token exists in the cache, if it doesn't exist, then it must not exist
        /// </summary>
        public bool BloomFilter { get; set; } = true;

        /// <summary>
        /// Unit: hour
        /// </summary>
        public int BloomFilterDataCleanInterval { get; set; } = 24 * 7;

        public string CachePrefix { get; set; } = "aspnetcore:auth:ref:{0}";
    }
}