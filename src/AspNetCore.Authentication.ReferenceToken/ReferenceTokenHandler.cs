using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using AspNetCore.Authentication.ReferenceToken.Events;
using AspNetCore.Authentication.ReferenceToken.Exceptions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenHandler: AuthenticationHandler<ReferenceTokenOptions>
    {
        private readonly ITokenValidator _tokenValidator;
        private readonly ReferenceTokenOptions _options;

        /// <summary>
        /// Initializes a new instance of <see cref="ReferenceTokenHandler"/>.
        /// </summary>
        /// <inheritdoc />
        public ReferenceTokenHandler(
            IOptionsMonitor<ReferenceTokenOptions> options, 
            ILoggerFactory logger,
            UrlEncoder encoder, 
            ISystemClock clock,
            ITokenValidator tokenValidator) : base(options, logger, encoder, clock)
        {
            _tokenValidator = tokenValidator;
            _options = options.CurrentValue;
        }

        /// <summary>
        /// The handler calls methods on the events which give the application control at certain points where processing is occurring. 
        /// If it is not provided a default instance is supplied which does nothing when the methods are called.
        /// </summary>
        protected new ReferenceTokenEvents Events
        {
            get => (ReferenceTokenEvents)base.Events;
            set => base.Events = value;
        }

        protected override Task<object> CreateEventsAsync() => Task.FromResult<object>(new ReferenceTokenEvents());

        protected virtual string ParseToken()
        {
            string token;
            var type = _options.ParseType;
            
            var attr = Context.GetEndpoint()?.Metadata.GetMetadata<TokenParseAttribute>();
            if (attr != null)
            {
                type = attr.Type;
            }
            
            switch (type)
            {
                case TokenParseType.Header:
                    token = ParseTokenFromHeader();
                    break;
                case TokenParseType.QueryString:
                    token = ParseTokenFromQueryString();
                    break;
                default:
                {
                    token = ParseTokenFromQueryString();
                    if (string.IsNullOrEmpty(token))
                    {
                        token = ParseTokenFromHeader();
                    }

                    break;
                }
            }

            if (token != null && token.Length != 64)
            {
                return null;
            }

            return token;
        }

        private string ParseTokenFromHeader()
        {
            string authorization = Request.Headers[Options.HeaderKey];

            if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authorization.Substring("Bearer ".Length).Trim();

            }

            return null;
        }

        private string ParseTokenFromQueryString()
        {
            if (Request.Query.ContainsKey(_options.QueryStringKey))
            {
                return Request.Query[_options.QueryStringKey];
            }

            return null;
        }

        /// <summary>
        /// Searches the 'Authorization' header for a 'Bearer' token.
        /// </summary>
        /// <returns></returns>
        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            try
            {
                // Give application opportunity to find from a different location, adjust, or reject token
                var messageReceivedContext = new MessageReceivedContext(Context, Scheme, Options);

                // event can set the token
                await Events.MessageReceived(messageReceivedContext);
                if (messageReceivedContext.Result != null)
                {
                    return messageReceivedContext.Result;
                }

                // If application retrieved token from somewhere else, use that.
                var token = messageReceivedContext.Token;
                
                if (string.IsNullOrEmpty(token))
                {
                    token = ParseToken();
                }

                if (string.IsNullOrEmpty(token))
                {
                    return AuthenticateResult.NoResult();
                }

                Exception validationFailure = null;
                ClaimsPrincipal principal = null;
                try
                {
                    principal = _tokenValidator.ValidateToken(token);
                }
                catch (Exception ex)
                {
                    Logger.TokenValidationFailed(ex);

                    validationFailure = ex;
                }

                if (validationFailure != null)
                {
                    var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                    {
                        Exception = validationFailure
                    };
                
                    await Events.AuthenticationFailed(authenticationFailedContext);
                    
                    if (authenticationFailedContext.Result!=null)
                    {
                        return authenticationFailedContext.Result;
                    }
                    
                    return AuthenticateResult.Fail(authenticationFailedContext.Exception);
                }

                Logger.TokenValidationSucceeded();
                var tokenValidatedContext = new TokenValidatedContext(Context, Scheme, Options)
                {
                    Principal = principal,
                    Token = token
                };

                await Events.TokenValidated(tokenValidatedContext);
                if (tokenValidatedContext.Result != null)
                {
                    return tokenValidatedContext.Result;
                }

                tokenValidatedContext.Success();
                return tokenValidatedContext.Result;
            }
            catch (Exception ex)
            {
                Logger.ErrorProcessingMessage(ex);

                var authenticationFailedContext = new AuthenticationFailedContext(Context, Scheme, Options)
                {
                    Exception = ex
                };

                await Events.AuthenticationFailed(authenticationFailedContext);
                
                if (authenticationFailedContext.Result!=null)
                {
                    return authenticationFailedContext.Result;
                }

                throw;
            }
        }
        
        protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            var authResult = await HandleAuthenticateOnceSafeAsync();
            var eventContext = new ReferenceTokenChallengeContext(Context, Scheme, Options, properties)
            {
                AuthenticateFailure = authResult?.Failure
            };

            // Avoid returning error=invalid_token if the error is not caused by an authentication failure (e.g missing token).
            if (Options.IncludeErrorDetails && eventContext.AuthenticateFailure != null)
            {
                eventContext.Error = "invalid_token";
                eventContext.ErrorDescription = CreateErrorDescription(eventContext.AuthenticateFailure);
            }

            await Events.Challenge(eventContext);
            if (eventContext.Handled)
            {
                return;
            }

            Response.StatusCode = 401;

            if (string.IsNullOrEmpty(eventContext.Error) &&
                string.IsNullOrEmpty(eventContext.ErrorDescription) &&
                string.IsNullOrEmpty(eventContext.ErrorUri))
            {
                Response.Headers.Append(HeaderNames.WWWAuthenticate, Options.Challenge);
            }
            else
            {
                // https://tools.ietf.org/html/rfc6750#section-3.1
                // WWW-Authenticate: Bearer realm="example", error="invalid_token", error_description="The access token expired"
                var builder = new StringBuilder(Options.Challenge);
                if (Options.Challenge.IndexOf(' ') > 0)
                {
                    // Only add a comma after the first param, if any
                    builder.Append(',');
                }
                if (!string.IsNullOrEmpty(eventContext.Error))
                {
                    builder.Append(" error=\"");
                    builder.Append(eventContext.Error);
                    builder.Append("\"");
                }
                if (!string.IsNullOrEmpty(eventContext.ErrorDescription))
                {
                    if (!string.IsNullOrEmpty(eventContext.Error))
                    {
                        builder.Append(",");
                    }

                    builder.Append(" error_description=\"");
                    builder.Append(eventContext.ErrorDescription);
                    builder.Append('\"');
                }
                if (!string.IsNullOrEmpty(eventContext.ErrorUri))
                {
                    if (!string.IsNullOrEmpty(eventContext.Error) ||
                        !string.IsNullOrEmpty(eventContext.ErrorDescription))
                    {
                        builder.Append(",");
                    }

                    builder.Append(" error_uri=\"");
                    builder.Append(eventContext.ErrorUri);
                    builder.Append('\"');
                }

                Response.Headers.Append(HeaderNames.WWWAuthenticate, builder.ToString());
            }
        }
        
        protected override Task HandleForbiddenAsync(AuthenticationProperties properties)
        {
            var forbiddenContext = new ForbiddenContext(Context, Scheme, Options);
            Response.StatusCode = 403;
            return Events.Forbidden(forbiddenContext);
        }

        private static string CreateErrorDescription(Exception authFailure)
        {
            string message = authFailure switch
            {
                ReferenceTokenExpiredException rte =>
                    $"The token expired at '{rte.ExpireAt.ToString(CultureInfo.InvariantCulture)}'",
                _ => authFailure.Message
            };

            return message;
        }

        
    }
}