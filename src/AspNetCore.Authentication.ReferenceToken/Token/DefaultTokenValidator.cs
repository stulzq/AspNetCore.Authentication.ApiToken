using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using AspNetCore.Authentication.ReferenceToken.Exceptions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class DefaultTokenValidator : ITokenValidator
    {
        private readonly ReferenceTokenOptions _options;
        private readonly ITokenStore _store;
        private readonly ITokenCacheService _cacheService;

        public DefaultTokenValidator(IOptions<ReferenceTokenOptions> options, ITokenStore store, ITokenCacheService cacheService)
        {
            _options = options.Value;
            _store = store;
            _cacheService = cacheService;
        }
        public virtual async Task<ClaimsPrincipal> ValidateTokenAsync([NotNull] string token, string schemeName)
        {
            TokenModel tokenModel = null;

            //Get from cache
            if (_options.UseCache)
            {
                var tokenCache = await _cacheService.GetAsync(token);

                if (tokenCache != null)
                {
                    if (!tokenCache.Available)
                    {
                        throw new TokenInvalidException(tokenCache.Reason);
                    }

                    tokenModel = tokenCache.Token;
                }
            }

            //If cache return null, then get from db
            if (tokenModel == null)
            {
                tokenModel = await _store.GetAsync(token);
                if (tokenModel != null && _options.UseCache)
                {
                    await _cacheService.SetAsync(tokenModel);
                }
            }

            if (tokenModel == null)
            {
                throw new TokenInvalidException("invalid token");
            }

            //Check expiration
            if (tokenModel.CheckExpiration())
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