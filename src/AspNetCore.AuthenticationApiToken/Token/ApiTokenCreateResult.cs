namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCreateResult
    {
        public bool Error { get; set; }

        public string ErrorDescription { get; set; }

        public ApiToken Token { get; set; }
        public ApiToken Refresh { get; set; }

        public static ApiTokenCreateResult Success(ApiToken token, ApiToken refreshToken) => new ApiTokenCreateResult() { Error = false, Token = token, Refresh = refreshToken };
        public static ApiTokenCreateResult Failed(string errorDescription) => new ApiTokenCreateResult() { Error = true, ErrorDescription = errorDescription };

    }
}