using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using AspNetCore.Authentication.ApiToken.Exceptions;

namespace AspNetCore.Authentication.ApiToken
{
    public class DefaultApiTokenValidator : IApiTokenValidator
    {
        private readonly IApiTokenStore _store;
        private readonly IApiTokenCacheService _cacheService;

        public DefaultApiTokenValidator(IApiTokenStore store, IApiTokenCacheService cacheService)
        {
            _store = store;
            _cacheService = cacheService;
        }

        public virtual async Task<ClaimsPrincipal> ValidateTokenAsync(ApiTokenOptions options, [NotNull] string token, string schemeName)
        {
            TokenModel tokenModel = null;

            //Get from cache
            if (options.UseCache)
            {
                var tokenModelCache = await _cacheService.GetAsync(token, schemeName);
                if (tokenModelCache != null)
                {
                    if (tokenModelCache.Available)
                    {
                        tokenModel = tokenModelCache.Token;
                    }
                    else
                    {
                        throw new TokenInvalidException("matching token could not be found from cache");
                    }
                }
            }

            //If cache return null, then get from db
            if (tokenModel == null)
            {
                var queryTokenModel = await _store.GetAsync(token, schemeName);
                if (queryTokenModel != null && queryTokenModel.IsValid)
                    tokenModel = queryTokenModel;

                //set cache
                if (tokenModel != null && options.UseCache)
                {
                    await _cacheService.SetAsync(tokenModel);
                }
            }

            if (tokenModel == null)
            {
                if (options.UseCache)
                {
                    await _cacheService.SetNullAsync(token, schemeName);
                }

                throw new TokenInvalidException("matching token could not be found");
            }

            //Check expiration
            if (!tokenModel.IsValid)
            {
                throw new TokenExpiredException("Token expired at " + tokenModel.Expiration, tokenModel.Expiration.DateTime);
            }

            //Generate ClaimsPrincipal
            var claims = tokenModel.Claims;

            var result = new ClaimsPrincipal();
            result.AddIdentity(new ClaimsIdentity(claims, schemeName, options.NameClaimType, options.RoleClaimType));

            return result;
        }
    }
}