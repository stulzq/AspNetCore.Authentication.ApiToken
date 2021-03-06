﻿using System;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.AspNetCore.Authentication;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Defines whether the token validation errors should be returned to the caller.
        /// Enabled by default, this option can be disabled to prevent the JWT handler
        /// from returning an error and an error_description in the WWW-Authenticate header.
        /// </summary>
        public bool IncludeErrorDetails { get; set; } = true;

        /// <summary>
        /// Gets or sets the challenge to put in the "WWW-Authenticate" header.
        /// </summary>
        public string Challenge { get; set; } = ApiTokenDefaults.AuthenticationScheme;

        /// <summary>
        /// The object provided by the application to process events raised by the api key authentication middleware.
        /// The application may implement the interface fully, or it may create an instance of <see cref="ApiTokenEvents"/>
        /// and assign delegates only to the events it wants to process.
        /// </summary>
        public new ApiTokenEvents Events
        {
            get => (ApiTokenEvents)base.Events;
            set => base.Events = value;
        }

        /// <summary>
        /// Set where to parse the token. Default <see cref="ApiTokenParseType.Both"/>
        /// </summary>
        public ApiTokenParseType ParseType { get; set; } = ApiTokenParseType.Both;

        /// <summary>
        /// Token parse from request header key. Default <see cref="ApiTokenDefaults.TokenParseHeaderKey"/>
        /// </summary>
        public string HeaderKey { get; set; } = ApiTokenDefaults.TokenParseHeaderKey;

        /// <summary>
        /// Token parse from request querystring key. Default <see cref="ApiTokenDefaults.TokenParseQueryStringKey"/>
        /// </summary>
        public string QueryStringKey { get; set; } = ApiTokenDefaults.TokenParseQueryStringKey;

        /// <summary>
        /// ApiToken expire time. Default: 1 hour.
        /// </summary>
        public TimeSpan TokenExpire { get; set; } = TimeSpan.FromHours(1);

        /// <summary>
        /// Refresh token expire time. Default: 24 hour.
        /// </summary>
        public TimeSpan RefreshTokenExpire { get; set; } = TimeSpan.FromHours(24);

        /// <summary>
        /// If set up to false,Repeated creation of token (<see cref="IApiTokenOperator.CreateAsync"/>) will invalidate the old token for user.
        /// </summary>
        public bool AllowMultiTokenActive { get; set; } = true;

        /// <summary>
        /// Use caching to improve performance, the cache service implementation interface <see cref="IApiTokenCacheService"/>.
        /// </summary>
        public bool UseCache { get; set; } = false;

        public string RoleClaimType { get; set; } = ApiTokenClaimTypes.Role;

        public string NameClaimType { get; set; } = ApiTokenClaimTypes.Name;

        /// <summary>
        /// The time when the old bearer token is still in effect when the token is refreshed. This property only valid when <see cref="ApiTokenOptions.AllowMultiTokenActive"/> = true.
        /// </summary>
        public TimeSpan? KeepTokenValidTimeSpanOnRefresh { get; set; }

    }
}