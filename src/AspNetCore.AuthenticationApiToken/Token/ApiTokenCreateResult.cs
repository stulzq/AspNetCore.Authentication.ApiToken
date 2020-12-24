namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCreateResult
    {
        public bool Error { get; set; }

        public string ErrorDescription { get; set; }

        public ApiTokenModel Token { get; set; }
        public ApiTokenModel Refresh { get; set; }

        public static ApiTokenCreateResult Success(ApiTokenModel token, ApiTokenModel refreshToken) => new ApiTokenCreateResult() { Error = false, Token = token, Refresh = refreshToken };
        public static ApiTokenCreateResult Failed(string errorDescription) => new ApiTokenCreateResult() { Error = true, ErrorDescription = errorDescription };

    }
}