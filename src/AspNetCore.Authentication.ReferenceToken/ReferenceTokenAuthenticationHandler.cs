using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenAuthenticationHandler: AuthenticationHandler<ReferenceTokenAuthenticationOptions>
    {
        public ReferenceTokenAuthenticationHandler(
            IOptionsMonitor<ReferenceTokenAuthenticationOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder, 
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}