using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ReferenceToken.Events
{
    /// <summary>
    /// A context for <see cref="ReferenceTokenEvents.OnMessageReceived"/>.
    /// </summary>
    public class MessageReceivedContext : ResultContext<ReferenceTokenOptions>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MessageReceivedContext"/>.
        /// </summary>
        /// <inheritdoc />
        public MessageReceivedContext(
            HttpContext context,
            AuthenticationScheme scheme,
            ReferenceTokenOptions options)
            : base(context, scheme, options) { }

        /// <summary>
        /// Bearer Token. This will give the application an opportunity to retrieve a token from an alternative location.
        /// </summary>
        public string Token { get; set; }

    }
}