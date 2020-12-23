using System;

namespace AspNetCore.Authentication.ReferenceToken
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TokenParseAttribute : Attribute
    {
        public TokenParseType ResolveType { get; }

        public TokenParseAttribute(TokenParseType resolveType = TokenParseType.Both)
        {
            ResolveType = resolveType;
        }
    }
}