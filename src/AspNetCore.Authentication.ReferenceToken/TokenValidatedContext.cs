using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ReferenceToken
{
    /// <summary>
    /// A context for <see cref="ReferenceTokenEvents.OnTokenValidated"/>.
    /// </summary>
    public class TokenValidatedContext : ResultContext<ReferenceTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="TokenValidatedContext"/>.
        /// </summary>
        /// <inheritdoc />
        public TokenValidatedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ReferenceTokenOptions options)
            : base(context, scheme, options) { }

    }
}