using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class TokenGenerateResult
    {
        public bool Error { get; set; }
        
        public string ErrorDescription { get; set; }
        
        public TokenModel Token { get; set; }
        
    }
}