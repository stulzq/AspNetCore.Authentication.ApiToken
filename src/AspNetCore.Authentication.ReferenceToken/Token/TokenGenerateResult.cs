using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenGenerateResult
    {
        public bool Error { get; set; }
        
        public string ErrorDescription { get; set; }
        
        public string TokenType { get; set; }
        
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
        
        public DateTime Expire { get; set; }
        
    }
}