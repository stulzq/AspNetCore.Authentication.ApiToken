using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ReferenceToken
{
    /// <summary>
    /// A <see cref="ResultContext{TOptions}"/> when authentication has failed.
    /// </summary>
    public class AuthenticationFailedContext : ResultContext<ReferenceTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="AuthenticationFailedContext"/>.
        /// </summary>
        /// <inheritdoc />
        public AuthenticationFailedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ReferenceTokenOptions options)
            : base(context, scheme, options) { }

        /// <summary>
        /// Gets or sets the exception associated with the authentication failure.
        /// </summary>
        public Exception Exception { get; set; }
    }
}