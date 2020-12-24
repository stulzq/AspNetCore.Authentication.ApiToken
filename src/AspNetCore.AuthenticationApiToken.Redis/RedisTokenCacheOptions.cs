using System;

namespace AspNetCore.Authentication.ApiToken.Redis
{
    public class RedisTokenCacheOptions
    {
        public string ConnectionString { get; set; }

        public TimeSpan? PreventPenetration { get; set; } = TimeSpan.FromMinutes(10);

        public string CachePrefix { get; set; } = "aspnetcore:auth:ref:{0}";
    }
}