using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;

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
    }
}