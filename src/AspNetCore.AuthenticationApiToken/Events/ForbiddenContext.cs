using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ApiToken.Events
{
    /// <summary>
    /// A <see cref="ResultContext{TOptions}"/> when access to a resource is forbidden.
    /// </summary>
    public class ForbiddenContext : ResultContext<ApiTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ForbiddenContext"/>.
        /// </summary>
        /// <inheritdoc />
        public ForbiddenContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ApiTokenOptions options)
            : base(context, scheme, options) { }
        

    }
}