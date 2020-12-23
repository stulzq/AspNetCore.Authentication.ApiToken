using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ReferenceToken.Events
{
    /// <summary>
    /// A <see cref="PropertiesContext{TOptions}"/> when access to a resource authenticated using Reference Token bearer is challenged.
    /// </summary>
    public class ReferenceTokenChallengeContext : PropertiesContext<ReferenceTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReferenceTokenChallengeContext"/>.
        /// </summary>
        /// <inheritdoc />
        public ReferenceTokenChallengeContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ReferenceTokenOptions options,
            AuthenticationProperties properties)
            : base(context, scheme, options, properties) { }

        /// <summary>
        /// Any failures encountered during the authentication process.
        /// </summary>
        public Exception AuthenticateFailure { get; set; }

        /// <summary>
        /// Gets or sets the "error" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// <see cref="ReferenceTokenOptions.IncludeErrorDetails"/> is set to <c>false</c>.
        /// </summary>
        public string Error { get; set; }

        /// <summary>
        /// Gets or sets the "error_description" value returned to the caller as part
        /// of the WWW-Authenticate header. This property may be null when
        /// <see cref="ReferenceTokenOptions.IncludeErrorDetails"/> is set to <c>false</c>.
        /// </summary>
        public string ErrorDescription { get; set; }

        /// <summary>
        /// Gets or sets the "error_uri" value returned to the caller as part of the
        /// WWW-Authenticate header. This property is always null unless explicitly set.
        /// </summary>
        public string ErrorUri { get; set; }

        /// <summary>
        /// If true, will skip any default logic for this challenge.
        /// </summary>
        public bool Handled { get; private set; }

        /// <summary>
        /// Skips any default logic for this challenge.
        /// </summary>
        public void HandleResponse() => Handled = true;
    }
}