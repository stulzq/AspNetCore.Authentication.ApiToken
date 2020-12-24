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
        

    }
}