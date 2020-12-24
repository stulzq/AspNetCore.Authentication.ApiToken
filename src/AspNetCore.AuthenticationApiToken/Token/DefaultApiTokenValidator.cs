using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using AspNetCore.Authentication.ApiToken.Exceptions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ApiToken
{
    public class DefaultApiTokenValidator : IApiTokenValidator
    {
        private readonly ApiTokenOptions _options;
        private readonly IApiTokenStore _store;
        private readonly IApiTokenCacheService _cacheService;

        public DefaultApiTokenValidator(IOptions<ApiTokenOptions> options, IApiTokenStore store, IApiTokenCacheService cacheService)
        {
            _options = options.Value;
            _store = store;
            _cacheService = cacheService;
        }
        public virtual async Task<ClaimsPrincipal> ValidateTokenAsync([NotNull] string token, string schemeName)
        {
            ApiToken tokenModel = null;

            //Get from cache
            if (_options.UseCache)
            {
                var tokenCache = await _cacheService.GetAsync(token);

                if (tokenCache != null)
                {
                    if (!tokenCache.Available)
                    {
                        throw new TokenInvalidException(tokenCache.Reason ?? "invalid_token");
                    }

                    tokenModel = tokenCache.Token;
                }
            }

            //If cache return null, then get from db
            if (tokenModel == null)
            {
                tokenModel = await _store.GetAsync(token);

                //set cache
                if (tokenModel != null && _options.UseCache)
                {
                    await _cacheService.SetAsync(tokenModel);
                }
            }

            if (tokenModel == null)
            {
                if (_options.UseCache)
                {
                    await _cacheService.SetNullAsync(token);
                }

                throw new TokenInvalidException("invalid_token");
            }

            //Check expiration
            if (tokenModel.IsExpired(_options.TokenExpireClockSkew))
            {
                throw new TokenExpiredException(tokenModel.Expiration.DateTime);
            }

            //Generate ClaimsPrincipal
            var claims = tokenModel.Claims;

            var result = new ClaimsPrincipal();
            result.AddIdentity(new ClaimsIdentity(claims, schemeName));
            return result;
        }
    }
}