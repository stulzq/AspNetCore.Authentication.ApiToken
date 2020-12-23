namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public interface ITokenValidator
    {
        void Validate(string token);
    }
}