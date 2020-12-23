using System;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken
{
    /// <summary>
    /// Used to setup defaults for all <see cref="ReferenceTokenOptions"/>.
    /// </summary>
    public class ReferenceTokenPostConfigureOptions : IPostConfigureOptions<ReferenceTokenOptions>
    {
        /// <summary>
        /// Invoked to post configure a JwtBearerOptions instance.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options instance to configure.</param>
        public void PostConfigure(string name, ReferenceTokenOptions options)
        {
            switch (options.ParseType)
            {
                case TokenParseType.Header:
                    ValidateHeaderKey(options);
                    break;
                case TokenParseType.QueryString:
                    ValidateQueryStringKey(options);
                    break;
                default:
                    ValidateHeaderKey(options);
                    ValidateQueryStringKey(options);
                    break;
            }
        }

        private static void ValidateHeaderKey(ReferenceTokenOptions options)
        {
            if (options.ParseType == TokenParseType.Header && string.IsNullOrEmpty(options.HeaderKey))
            {
                throw new InvalidOperationException($"{nameof(ReferenceTokenOptions.HeaderKey)} must be set.");
            }
        }

        private static void ValidateQueryStringKey(ReferenceTokenOptions options)
        {
            if (options.ParseType == TokenParseType.QueryString && string.IsNullOrEmpty(options.QueryStringKey))
            {
                throw new InvalidOperationException($"{nameof(ReferenceTokenOptions.QueryStringKey)} must be set.");
            }
        }
    }
}