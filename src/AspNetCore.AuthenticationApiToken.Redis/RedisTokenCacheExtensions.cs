using System;
using AspNetCore.Authentication.ApiToken.Redis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace AspNetCore.Authentication.ApiToken
{
    public static class RedisTokenCacheExtensions
    {
        public static ApiTokenAuthenticationBuilder AddRedisCache(this ApiTokenAuthenticationBuilder builder, Action<RedisTokenCacheOptions> configureOptions)
        {
            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<RedisTokenCacheOptions>, RedisTokenCachePostConfigureOptions>());
            builder.AddCache<RedisTokenCacheService, RedisTokenCacheOptions>(configureOptions);
            
            return builder;
        }
    }
}