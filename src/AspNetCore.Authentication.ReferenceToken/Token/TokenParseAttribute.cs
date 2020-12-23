using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TokenParseAttribute : Attribute
    {
        public TokenParseType Type { get; }

        public TokenParseAttribute(TokenParseType type = TokenParseType.Both)
        {
            Type = type;
        }
    }
}