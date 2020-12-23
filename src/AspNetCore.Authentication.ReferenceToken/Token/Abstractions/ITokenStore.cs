namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenStore
    {
        void Save();

        void Get();
    }
}