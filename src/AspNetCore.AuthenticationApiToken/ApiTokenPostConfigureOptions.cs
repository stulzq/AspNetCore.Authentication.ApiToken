using System;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ApiToken
{
    /// <summary>
    /// Used to setup defaults for all <see cref="ApiTokenOptions"/>.
    /// </summary>
    public class ApiTokenPostConfigureOptions : IPostConfigureOptions<ApiTokenOptions>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, ApiTokenOptions options)
        {
            switch (options.ParseType)
            {
                case ApiTokenParseType.Header:
                    ValidateHeaderKey(options);
                    break;
                case ApiTokenParseType.QueryString:
                    ValidateQueryStringKey(options);
                    break;
                default:
                    ValidateHeaderKey(options);
                    ValidateQueryStringKey(options);
                    break;
            }
        }

        private static void ValidateHeaderKey(ApiTokenOptions options)
        {
            if (options.ParseType == ApiTokenParseType.Header && string.IsNullOrEmpty(options.HeaderKey))
            {
                throw new InvalidOperationException($"{nameof(ApiTokenOptions.HeaderKey)} must be set.");
            }
        }

        private static void ValidateQueryStringKey(ApiTokenOptions options)
        {
            if (options.ParseType == ApiTokenParseType.QueryString && string.IsNullOrEmpty(options.QueryStringKey))
            {
                throw new InvalidOperationException($"{nameof(ApiTokenOptions.QueryStringKey)} must be set.");
            }
        }
    }
}