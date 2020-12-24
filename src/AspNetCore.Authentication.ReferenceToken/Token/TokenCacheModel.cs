namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenCacheModel
    {
        public TokenModel Token { get; set; }
        public bool Available => Token != null;
        public string Reason { get; set; }
    }
}