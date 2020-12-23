using Microsoft.AspNetCore.Authentication;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenAuthenticationOptions: AuthenticationSchemeOptions
    {
        /// <summary>
        /// Set where to parse the token. Default <see cref="TokenParseType.Both"/>
        /// </summary>
        public TokenParseType ParseType { get; set; }

        /// <summary>
        /// Token parse from request header key. Default <see cref="ReferenceTokenDefaults.TokenParseHeaderKey"/>
        /// </summary>
        public string HeaderKey { get; set; }

        /// <summary>
        /// Token parse from request querystring key. Default <see cref="ReferenceTokenDefaults.TokenParseQueryStringKey"/>
        /// </summary>
        public string QueryStringKey { get; set; }

        /// <summary>
        /// Default value is false.
        /// When set to true, it will NOT return WWW-Authenticate response header when challenging un-authenticated requests.
        /// When set to false, it will return WWW-Authenticate response header when challenging un-authenticated requests.
        /// It is normally used to disable browser prompt when doing ajax calls.
        /// ReSharper disable once InconsistentNaming
        /// <see href="https://tools.ietf.org/html/rfc7235#section-4.1"/>
        /// </summary>
        public bool SuppressWWWAuthenticateHeader { get; set; } = false;
    }
}