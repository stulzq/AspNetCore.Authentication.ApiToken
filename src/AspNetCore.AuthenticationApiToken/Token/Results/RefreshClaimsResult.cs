namespace AspNetCore.Authentication.ApiToken
{
    public class RefreshClaimsResult : ResultBase
    {
        public static RefreshClaimsResult Success() => new RefreshClaimsResult() { Error = false };
        public static RefreshClaimsResult Failed(string errorDescription) => new RefreshClaimsResult() { Error = true, ErrorDescription = errorDescription };
    }
}