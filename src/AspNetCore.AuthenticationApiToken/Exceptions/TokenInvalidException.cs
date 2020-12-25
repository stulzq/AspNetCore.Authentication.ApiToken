using System;

namespace AspNetCore.Authentication.ApiToken.Exceptions
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException()
        {
            
        }
        public TokenInvalidException(string message) : base(message)
        {

        }
    }
}