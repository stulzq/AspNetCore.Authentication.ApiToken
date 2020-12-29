namespace AspNetCore.Authentication.ApiToken
{
    public class TokenModelCache
    {
        public TokenModel Token { get; set; }
        public bool Available => Token != null;
    }
}