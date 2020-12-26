using System;

namespace AspNetCore.Authentication.ApiToken.Exceptions
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException(string message) : base(message)
        {

        }
    }
}