namespace AspNetCore.Authentication.ApiToken
{
    public class TokenCreateResult : ResultBase
    {
        public TokenModel Bearer { get; set; }
        public TokenModel Refresh { get; set; }

        public static TokenCreateResult Success(TokenModel token, TokenModel refreshToken) => new TokenCreateResult() { Error = false, Bearer = token, Refresh = refreshToken };
        public static TokenCreateResult Failed(string errorDescription) => new TokenCreateResult() { Error = true, ErrorDescription = errorDescription };

    }
}