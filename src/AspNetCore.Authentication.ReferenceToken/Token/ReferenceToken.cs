using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class ReferenceToken
    {
        public DateTime CreateTime { get; set; }
        
        public DateTime ExpireTime { get; set; }
        
        public string Token { get; set; }
        
        public string RefreshToken { get; set; }
    }
}