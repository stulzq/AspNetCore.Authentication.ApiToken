using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace AspNetCore.Authentication.ApiToken
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