namespace AspNetCore.Authentication.ReferenceToken
{
    public static class ReferenceTokenDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme 
        /// </summary>
        public const string AuthenticationScheme = "Reference";

        /// <summary>
        /// Get token from request header key
        /// </summary>
        public const string TokenParseHeaderKey = "Authorization";

        /// <summary>
        /// Get token from request querystring key, eg. https://www.google.com/api/apple?token=xxxx
        /// </summary>
        public const string TokenParseQueryStringKey = "Token";
    }
}