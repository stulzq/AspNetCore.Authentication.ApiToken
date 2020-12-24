using System;

namespace AspNetCore.Authentication.ReferenceToken.Exceptions
{
    public class TokenInvalidException : Exception
    {
        public TokenInvalidException(string message) : base(message)
        {

        }
    }
}