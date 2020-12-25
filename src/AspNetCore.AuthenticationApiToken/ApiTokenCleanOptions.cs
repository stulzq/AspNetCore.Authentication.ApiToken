using AspNetCore.Authentication.ApiToken.Abstractions;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCleanOptions
    {
        /// <summary>
        /// Background Service periodically run clean stored expired token, will call <see cref="IApiTokenStore.RemoveExpirationAsync"/>. Unit: second.
        /// <para></para>
        /// * If set value to 0, the service will not start.
        /// </summary>
        public int Interval { get; set; } = 86400;
    }
}