using System;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ApiToken.Redis
{
    /// <summary>
    /// Used to setup defaults for all <see cref="RedisTokenCacheOptions"/>.
    /// </summary>
    public class RedisTokenCachePostConfigureOptions : IPostConfigureOptions<RedisTokenCacheOptions>
    {
        public void PostConfigure(string name, RedisTokenCacheOptions options)
        {
            if (string.IsNullOrEmpty(options.ConnectionString))
            {
                throw new InvalidOperationException($"{nameof(RedisTokenCacheOptions.ConnectionString)} must be not null.");
            }
        }
    }
}