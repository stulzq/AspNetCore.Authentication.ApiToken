using System;

namespace AspNetCore.Authentication.ApiToken
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ApiTokenParseAttribute : Attribute
    {
        public ApiTokenParseType Type { get; }

        public ApiTokenParseAttribute(ApiTokenParseType type = ApiTokenParseType.Both)
        {
            Type = type;
        }
    }
}