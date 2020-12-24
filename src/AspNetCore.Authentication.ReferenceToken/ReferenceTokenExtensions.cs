using System;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken
{
    /// <summary>
    /// Extension methods to configure Reference Token bearer authentication.
    /// </summary>
    public static class ReferenceTokenExtensions
    {
        /// <summary>
        /// Enables Reference Token-bearer authentication using the default scheme <see cref="AuthenticationScheme"/>.
        /// <para>
        /// Reference Token bearer authentication performs authentication by extracting and validating a Reference Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddReferenceToken(this AuthenticationBuilder builder)
            => builder.AddReferenceToken(ReferenceTokenDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Enables Reference Token-bearer authentication using the default scheme <see cref="AuthenticationScheme"/>.
        /// <para>
        /// Reference Token bearer authentication performs authentication by extracting and validating a Reference Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ReferenceTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddReferenceToken(this AuthenticationBuilder builder, Action<ReferenceTokenOptions> configureOptions)
            => builder.AddReferenceToken(ReferenceTokenDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Enables Reference Token-bearer authentication using the specified scheme.
        /// <para>
        /// Reference Token bearer authentication performs authentication by extracting and validating a Reference Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ReferenceTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddReferenceToken(this AuthenticationBuilder builder, string authenticationScheme, Action<ReferenceTokenOptions> configureOptions)
            => builder.AddReferenceToken(authenticationScheme, displayName: null, configureOptions: configureOptions);

        /// <summary>
        /// Enables Reference Token-bearer authentication using the specified scheme.
        /// <para>
        /// Reference Token bearer authentication performs authentication by extracting and validating a Reference Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ReferenceTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static AuthenticationBuilder AddReferenceToken(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ReferenceTokenOptions> configureOptions)
        {
            builder.Services.AddSingleton<ITokenCacheService, NullTokenCacheService>();
            builder.Services.AddTransient<ITokenValidator, DefaultTokenValidator>();
            builder.Services.AddTransient<ITokenOperator, DefaultTokenOperator>();

            //store profile

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ReferenceTokenOptions>, ReferenceTokenPostConfigureOptions>());
            return builder.AddScheme<ReferenceTokenOptions, ReferenceTokenHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}