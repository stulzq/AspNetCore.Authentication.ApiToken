using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenCacheOptions
    {
        public string ConnectionString { get; set; }

        public TimeSpan? PreventPenetration { get; set; } = TimeSpan.FromHours(1);

        public bool BloomFilter { get; set; } = true;

        public string CachePrefix { get; set; } = "aspnetcore:auth:ref:{0}";
    }
}