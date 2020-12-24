using System;
using System.Security.Claims;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenModel
    {
        public string Token { get; set; }

        /// <summary>
        /// reference or refresh
        /// </summary>
        public string Type { get; set; }

        public string UserId { get; set; }

        public Claim[] Claims { get; set; }

        public DateTime CreateTime { get; set; }
        
        public DateTime Expiration { get; set; }
        
    }
}