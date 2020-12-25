using System;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenAuthenticationBuilder
    {
        public IServiceCollection Services { get; }

        public ApiTokenAuthenticationBuilder(AuthenticationBuilder builder)
        {
            Services = builder.Services;
        }

        public ApiTokenAuthenticationBuilder AddProfileService<TProfileService>() where TProfileService : class
        {
            Services.AddTransient(typeof(IApiTokenProfileService), typeof(TProfileService));
            return this;
        }

        public ApiTokenAuthenticationBuilder AddTokenStore<TTokenStore>() where TTokenStore : class
        {
            Services.AddScoped(typeof(IApiTokenStore), typeof(TTokenStore));
            return this;
        }

        public ApiTokenAuthenticationBuilder AddCache<TCacheService, TCacheOptions>(Action<TCacheOptions> configureOptions)
            where TCacheService : IApiTokenCacheService
            where TCacheOptions : ApiTokenCacheOptions
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            Services.Configure(configureOptions);
            Services.Replace(ServiceDescriptor.Singleton(typeof(IApiTokenCacheService), typeof(TCacheService)));
            return this;
        }

        public ApiTokenAuthenticationBuilder AddCleanService() => AddCleanService(_ => { });
        
        public ApiTokenAuthenticationBuilder AddCleanService(Action<ApiTokenCleanOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            Services.Configure(configureOptions);
            Services.AddHostedService<ApiTokenCleanHostedService>();
            return this;
        }
    }
}