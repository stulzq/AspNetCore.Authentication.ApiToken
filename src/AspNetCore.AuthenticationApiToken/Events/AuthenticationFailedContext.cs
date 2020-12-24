using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ApiToken.Events
{
    /// <summary>
    /// A <see cref="ResultContext{TOptions}"/> when authentication has failed.
    /// </summary>
    public class AuthenticationFailedContext : ResultContext<ApiTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationFailedContext"/>.
        /// </summary>
        /// <inheritdoc />
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ApiTokenOptions options)
            : base(context, scheme, options) { }

        /// <summary>
        /// Gets or sets the exception associated with the authentication failure.
        /// </summary>
        public Exception Exception { get; set; }
    }
}