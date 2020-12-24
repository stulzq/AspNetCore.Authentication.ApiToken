using System;

namespace AspNetCore.Authentication.ReferenceToken.Exceptions
{
    public class ReferenceTokenExpiredException : Exception
    {
        public DateTimeOffset ExpireAt { get; }

        public ReferenceTokenExpiredException(DateTime expireAt)
        {
            ExpireAt = expireAt;
        }

    }
}