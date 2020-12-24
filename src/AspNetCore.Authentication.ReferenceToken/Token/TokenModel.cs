using System;
using System.Security.Claims;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenModel
    {
        public string Token { get; set; }

        /// <summary>
        /// reference or refresh <see cref="TokenType"/>
        /// </summary>
        public TokenType Type { get; set; }

        public string UserId { get; set; }

        public Claim[] Claims { get; set; }

        public DateTimeOffset CreateTime { get; set; }
        
        public DateTimeOffset Expiration { get; set; }

        public bool CheckExpiration()
        {
            return Expiration.UtcDateTime < DateTimeOffset.UtcNow;
        }
        
    }
}