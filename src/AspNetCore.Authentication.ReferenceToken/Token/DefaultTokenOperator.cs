using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken
{
    public class DefaultTokenOperator : ITokenOperator
    {
        private readonly ReferenceTokenOptions _options;
        private readonly IProfileService _profileService;
        private readonly ITokenStore _tokenStore;
        private readonly ITokenCacheService _cacheService;

        public DefaultTokenOperator(IOptions<ReferenceTokenOptions> options,
            IProfileService profileService,
            ITokenStore tokenStore,
            ITokenCacheService cacheService)
        {
            _options = options.Value;
            _profileService = profileService;
            _tokenStore = tokenStore;
            _cacheService = cacheService;
        }

        public virtual async Task<TokenCreateResult> CreateAsync(string userId)
        {
            var claims = await _profileService.GetUserClaimsAsync(userId);
            var result = CreateByUserClaims(userId, claims);

            await RemoveOldTokenAsync(userId);

            await _tokenStore.StoreAsync(new List<TokenModel>() { result.Token, result.Refresh });

            if (_options.UseCache)
            {
                await _cacheService.SetAsync(result.Token);
            }

            return result;
        }

        private TokenCreateResult CreateByUserClaims(string userId, Claim[] claims)
        {
            var now = DateTime.Now;
            var token = new TokenModel()
            {
                Token = TokenTools.CreateToken(userId),
                CreateTime = now,
                Type = TokenType.Reference,
                UserId = userId,
                Claims = claims,
                Expiration = now + _options.TokenExpire
            };

            var refreshToken = new TokenModel()
            {
                Token = TokenTools.CreateToken(userId),
                CreateTime = now,
                Type = TokenType.Refresh,
                UserId = userId,
                Claims = null,
                Expiration = now + _options.RefreshTokenExpire
            };

            return TokenCreateResult.Success(token, refreshToken);
        }

        private async Task RemoveOldTokenAsync(string userId)
        {
            if (!_options.AllowMultiTokenActive)
            {
                const string reason = "Only one token active at the same time is allowed.";

                //Remove old token from cache
                if (_options.UseCache)
                {
                    var tokenList = await _tokenStore.GetListAsync(userId);
                    foreach (var tokenItem in tokenList)
                    {
                        await _cacheService.RemoveAsync(tokenItem, reason);
                    }
                }

                //Remove old token from db
                await _tokenStore.RemoveListAsync(userId);
            }
        }

        public virtual async Task<TokenCreateResult> RefreshAsync(string refreshToken)
        {
            var token = await _tokenStore.GetAsync(refreshToken);
            if (token == null || token.Type != TokenType.Refresh)
            {
                return TokenCreateResult.Failed("invalid refresh_token");
            }

            if (token.IsExpired(_options.TokenExpireClockSkew))
            {
                return TokenCreateResult.Failed($"The refresh_token expired at '{token.Expiration.LocalDateTime.ToString(CultureInfo.InvariantCulture)}'");
            }

            var claims = await _profileService.GetUserClaimsAsync(token.UserId);
            var result = CreateByUserClaims(token.UserId, claims);

            await RemoveOldTokenAsync(token.UserId);

            await _tokenStore.StoreAsync(new List<TokenModel>() { result.Token, result.Refresh });

            if (_options.UseCache)
            {
                await _cacheService.SetAsync(result.Token);
            }

            return result;
        }

        public virtual async Task<RefreshClaimsResult> RefreshClaimsAsync(string token)
        {
            var tokenModel = await _tokenStore.GetAsync(token);
            if (tokenModel == null || tokenModel.Type != TokenType.Reference)
            {
                return RefreshClaimsResult.Failed("invalid token");
            }

            var claims = await _profileService.GetUserClaimsAsync(tokenModel.UserId);

            //Refresh db
            await _tokenStore.UpdateClaimsAsync(token, claims);

            //Refresh cache
            if (_options.UseCache)
            {
                tokenModel.Claims = claims;
                await _cacheService.SetAsync(tokenModel);
            }

            return RefreshClaimsResult.Success();
        }

        public virtual async Task RemoveAsync(string token, string reason = null)
        {
            var tokenModel = await _tokenStore.GetAsync(token);
            if (tokenModel == null)
            {
                return;
            }

            //Remove from cache
            if (_options.UseCache)
            {
                await _cacheService.RemoveAsync(tokenModel, reason);
            }

            //Remove from db
            await _tokenStore.RemoveAsync(token);
        }
    }
}