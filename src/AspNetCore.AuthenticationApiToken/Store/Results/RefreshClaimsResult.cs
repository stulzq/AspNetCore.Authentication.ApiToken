namespace AspNetCore.Authentication.ApiToken.Results
{
    public class RefreshClaimsResult : ResultBase
    {
        public static RefreshClaimsResult Success() => new RefreshClaimsResult() { Error = false };
        public static RefreshClaimsResult Failed(string errorDescription) => new RefreshClaimsResult() { Error = true, ErrorDescription = errorDescription };
    }
}