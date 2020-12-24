using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ApiToken.Events
{
    /// <summary>
    /// A context for <see cref="ApiTokenEvents.OnTokenValidated"/>.
    /// </summary>
    public class ApiTokenValidatedContext : ResultContext<ApiTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ApiTokenValidatedContext"/>.
        /// </summary>
        /// <inheritdoc />
        public ApiTokenValidatedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ApiTokenOptions options)
            : base(context, scheme, options) { }

        public string Token { get; set; }

    }
}