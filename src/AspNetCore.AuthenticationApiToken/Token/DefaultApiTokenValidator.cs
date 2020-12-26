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
                var tokenCache = await _cacheService.GetAsync(token);

                if (tokenCache != null)
                {
                    if (!tokenCache.Available)
                    {
                        if (tokenCache.Reason == ApiTokenGlobalSettings.Reason.NotAllowMultiTokenActive)
                        {
                            throw new TokenMultiActiveException(tokenCache.Reason);
                        }

                        throw new TokenInvalidException(tokenCache.Reason);
                    }

                    tokenModel = tokenCache.Token;
                }
            }

            //If cache return null, then get from db
            if (tokenModel == null)
            {
                tokenModel = await _store.GetAsync(token);

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
                    await _cacheService.SetNullAsync(token);
                }

                throw new TokenInvalidException("not found");
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