using System;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ApiToken
{
    /// <summary>
    /// Extension methods to configure ApiToken Token bearer authentication.
    /// </summary>
    public static class ApiTokenExtensions
    {
        /// <summary>
        /// Enables ApiToken Token-bearer authentication using the default scheme <see cref="AuthenticationScheme"/>.
        /// <para>
        /// ApiToken Token bearer authentication performs authentication by extracting and validating a ApiToken Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static ApiTokenAuthenticationBuilder AddApiToken(this AuthenticationBuilder builder)
            => builder.AddApiToken(ApiTokenDefaults.AuthenticationScheme, _ => { });

        /// <summary>
        /// Enables ApiToken Token-bearer authentication using the default scheme <see cref="AuthenticationScheme"/>.
        /// <para>
        /// ApiToken Token bearer authentication performs authentication by extracting and validating a ApiToken Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ApiTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static ApiTokenAuthenticationBuilder AddApiToken(this AuthenticationBuilder builder, Action<ApiTokenOptions> configureOptions)
            => builder.AddApiToken(ApiTokenDefaults.AuthenticationScheme, configureOptions);

        /// <summary>
        /// Enables ApiToken Token-bearer authentication using the specified scheme.
        /// <para>
        /// ApiToken Token bearer authentication performs authentication by extracting and validating a ApiToken Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ApiTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static ApiTokenAuthenticationBuilder AddApiToken(this AuthenticationBuilder builder, string authenticationScheme, Action<ApiTokenOptions> configureOptions)
            => builder.AddApiToken(authenticationScheme, displayName: null, configureOptions: configureOptions);

        /// <summary>
        /// Enables ApiToken Token-bearer authentication using the specified scheme.
        /// <para>
        /// ApiToken Token bearer authentication performs authentication by extracting and validating a ApiToken Token token from the <c>Authorization</c> request header.
        /// </para>
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme.</param>
        /// <param name="displayName">The display name for the authentication handler.</param>
        /// <param name="configureOptions">A delegate that allows configuring <see cref="ApiTokenOptions"/>.</param>
        /// <returns>A reference to <paramref name="builder"/> after the operation has completed.</returns>
        public static ApiTokenAuthenticationBuilder AddApiToken(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<ApiTokenOptions> configureOptions)
        {
            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }
            
            builder.Services.AddSingleton<IApiTokenCacheService, NullApiTokenCacheService>();
            builder.Services.AddTransient<IApiTokenValidator, DefaultApiTokenValidator>();
            builder.Services.AddTransient<IApiTokenOperator, DefaultApiTokenOperator>();

            builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<ApiTokenOptions>, ApiTokenPostConfigureOptions>());
            builder.AddScheme<ApiTokenOptions, ApiTokenHandler>(authenticationScheme, displayName, configureOptions);

            return new ApiTokenAuthenticationBuilder(builder);
        }
    }
}