using System;

namespace AspNetCore.Authentication.ApiToken.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public DateTimeOffset ExpireAt { get; }

        public TokenExpiredException(DateTime expireAt)
        {
            ExpireAt = expireAt;
        }

    }
}