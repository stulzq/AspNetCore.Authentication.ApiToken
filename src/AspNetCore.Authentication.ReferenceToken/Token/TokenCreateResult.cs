namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenCreateResult
    {
        public bool Error { get; set; }

        public string ErrorDescription { get; set; }

        public TokenModel Token { get; set; }
        public TokenModel Refresh { get; set; }

        public static TokenCreateResult Success(TokenModel token, TokenModel refreshToken) => new TokenCreateResult() { Error = false, Token = token, Refresh = refreshToken };
        public static TokenCreateResult Failed(string errorDescription) => new TokenCreateResult() { Error = true, ErrorDescription = errorDescription };

    }
}