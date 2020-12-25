using System;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCacheOptions
    {
        /// <summary>
        /// Redis connection string. Help: https://stackexchange.github.io/StackExchange.Redis/Configuration.html
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Set an empty cache for invalid token to avoid penetrating the database
        /// <para></para>
        /// * If set value to null, it will not take effect
        /// </summary>
        public TimeSpan? InvalidTokenNullCacheTTL { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Cache key prefix in redis.
        /// </summary>
        public string CachePrefix { get; set; } = "aspnetcore:authentication";

        /// <summary>
        /// The invalid token will be remove immediately by the token store,
        /// but he token's invalid reason can be cached and used to return to the client.
        /// This option represents the TTL of the cache for token's invalid reason.
        /// <para></para>
        /// * If set value to null, it will not take effect.
        /// </summary>
        public TimeSpan? InvalidTokenReasonCacheTTL { get; set; }
    }
}