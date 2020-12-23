namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenOperationContext
    {
        TokenGenerateResult Create();
        TokenGenerateResult Refresh();
        void RefreshClaims();
        void Revoke();
    }
}