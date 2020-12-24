using System;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceTokenOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Set where to parse the token. Default <see cref="TokenParseType.Both"/>
        /// </summary>
        public TokenParseType ParseType { get; set; }

        /// <summary>
        /// Token parse from request header key. Default <see cref="ReferenceTokenDefaults.TokenParseHeaderKey"/>
        /// </summary>
        public string HeaderKey { get; set; } = ReferenceTokenDefaults.TokenParseHeaderKey;

        /// <summary>
        /// Token parse from request querystring key. Default <see cref="ReferenceTokenDefaults.TokenParseQueryStringKey"/>
        /// </summary>
        public string QueryStringKey { get; set; } = ReferenceTokenDefaults.TokenParseQueryStringKey;

        /// <summary>
        /// Defines whether the token validation errors should be returned to the caller.
        /// Enabled by default, this option can be disabled to prevent the JWT handler
        /// from returning an error and an error_description in the WWW-Authenticate header.
        /// </summary>
        public bool IncludeErrorDetails { get; set; } = true;

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = ReferenceTokenDefaults.AuthenticationScheme;

        /// <summary>
        /// The object provided by the application to process events raised by the api key authentication middleware.
        /// The application may implement the interface fully, or it may create an instance of <see cref="ReferenceTokenEvents"/>
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new ReferenceTokenEvents Events
        {
            get => (ReferenceTokenEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// Reference token expire time. Default: 1 hour.
        /// </summary>
        public TimeSpan TokenExpire { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Refresh token expire time. Default: 24 hour.
        /// </summary>
        public TimeSpan RefreshTokenExpire { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// If set up to false,Repeated creation of token (<see cref="ITokenOperator.CreateAsync"/>) will invalidate the old token for one user.
        /// </summary>
        public bool AllowMultiTokenActive { get; set; } = true;

        /// <summary>
        /// Use cache <see cref="ITokenCacheService"/>
        /// </summary>
        public bool UseCache { get; set; } = false;

    }
}