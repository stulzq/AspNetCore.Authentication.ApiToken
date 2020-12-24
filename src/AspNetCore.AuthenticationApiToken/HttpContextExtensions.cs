using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken;
using Microsoft.AspNetCore.Authentication;

// ReSharper disable once CheckNamespace
namespace Microsoft.AspNetCore.Http
{
    public static class HttpContextExtensions
    {
        public static Task<string> GetApiTokenAsync(this HttpContext context)
        {
            return context.GetTokenAsync(ApiTokenDefaults.ApiTokenName);
        }

        public static Task<string> GetApiTokenAsync(this HttpContext context, string scheme)
        {
            return context.GetTokenAsync(scheme, ApiTokenDefaults.ApiTokenName);
        }
    }
}