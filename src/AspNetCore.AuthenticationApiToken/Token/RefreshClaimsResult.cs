namespace AspNetCore.Authentication.ApiToken
{
    public class RefreshClaimsResult
    {
        public bool Error { get; set; }

        public string ErrorDescription { get; set; }

        public static RefreshClaimsResult Success() => new RefreshClaimsResult() { Error = false};
        public static RefreshClaimsResult Failed(string errorDescription) => new RefreshClaimsResult() { Error = true, ErrorDescription = errorDescription };
    }
}