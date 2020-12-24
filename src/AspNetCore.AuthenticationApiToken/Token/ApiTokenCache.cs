namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCache
    {
        public ApiToken Token { get; set; }
        public bool Available => Token != null;
        public string Reason { get; set; }
    }
}