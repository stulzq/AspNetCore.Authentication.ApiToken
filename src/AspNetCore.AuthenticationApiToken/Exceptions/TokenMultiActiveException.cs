using System;

namespace AspNetCore.Authentication.ApiToken.Exceptions
{
    public class TokenMultiActiveException : Exception
    {
        public TokenMultiActiveException(string message) : base(message)
        {

        }
    }
}