using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace AspNetCore.Authentication.ApiToken
{
    public class TokenModel
    {
        public string Value { get; set; }

        /// <summary>
        /// Bearer or Refresh <see cref="TokenType"/>
        /// </summary>
        public TokenType Type { get; set; }

        public string UserId { get; set; }

        public IReadOnlyList<Claim> Claims { get; set; }

        public DateTimeOffset CreateTime { get; set; }

        public DateTimeOffset Expiration { get; set; }

        public bool IsValid => (Expiration.UtcDateTime - DateTimeOffset.UtcNow).TotalMilliseconds > 0;

        public TimeSpan LifeTime=> Expiration.UtcDateTime - DateTimeOffset.UtcNow;

    }
}