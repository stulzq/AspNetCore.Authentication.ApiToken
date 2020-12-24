namespace AspNetCore.Authentication.ApiToken
{
    public static class ApiTokenDefaults
    {
        /// <summary>
        /// Default value for AuthenticationScheme 
        /// </summary>
        public const string AuthenticationScheme = "ApiToken";

        /// <summary>
        /// Get token from request header key, eg. Authorization: Bearer xxx
        /// </summary>
        public const string TokenParseHeaderKey = "Authorization";

        /// <summary>
        /// Get token from request querystring key, eg. https://www.google.com/api/apple?token=xxxx
        /// </summary>
        public const string TokenParseQueryStringKey = "ApiToken";
        
        public const string ApiTokenName = "api_token";
    }
}