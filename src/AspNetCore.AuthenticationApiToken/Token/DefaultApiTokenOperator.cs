﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ApiToken
{
    public class DefaultApiTokenOperator : IApiTokenOperator
    {
        private readonly ApiTokenOptions _options;
        private readonly IApiTokenProfileService _profileService;
        private readonly IApiTokenStore _tokenStore;
        private readonly IApiTokenCacheService _cacheService;

        public DefaultApiTokenOperator(IOptions<ApiTokenOptions> options,
            IApiTokenProfileService profileService,
            IApiTokenStore tokenStore,
            IApiTokenCacheService cacheService)
        {
            _options = options.Value;
            _profileService = profileService;
            _tokenStore = tokenStore;
            _cacheService = cacheService;
        }

        public virtual async Task<TokenCreateResult> CreateAsync(string userId)
        {
            var claims = await GetUserClaimsAsync(userId);

            var result = CreateByUserClaims(userId, claims);

            await RemoveOldTokenAsync(userId);

            await _tokenStore.StoreAsync(new List<TokenModel>() { result.Bearer, result.Refresh });

            if (_options.UseCache)
            {
                await _cacheService.SetAsync(result.Bearer);
            }

            return result;
        }

        private TokenCreateResult CreateByUserClaims(string userId, List<Claim> claims)
        {
            var now = DateTime.Now;
            var token = new TokenModel()
            {
                Value = ApiTokenTools.CreateToken(userId),
                CreateTime = now,
                Type = TokenType.Bearer,
                UserId = userId,
                Claims = claims,
                Expiration = now + _options.TokenExpire
            };

            var refreshToken = new TokenModel()
            {
                Value = ApiTokenTools.CreateToken(userId),
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
                //Remove old token from cache
                if (_options.UseCache)
                {
                    var tokenList = await _tokenStore.GetListAsync(userId);
                    foreach (var token in tokenList)
                    {
                        await _cacheService.RemoveAsync(token.Value, ApiTokenGlobalSettings.Reason.NotAllowMultiTokenActive);
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

            if (!token.IsValid)
            {
                return TokenCreateResult.Failed($"The refresh_token expired at '{token.Expiration.LocalDateTime.ToString(CultureInfo.InvariantCulture)}'");
            }

            var claims = await GetUserClaimsAsync(token.UserId);
            var result = CreateByUserClaims(token.UserId, claims);

            await RemoveOldTokenAsync(token.UserId);
            await _tokenStore.RemoveAsync(refreshToken);

            await _tokenStore.StoreAsync(new List<TokenModel>() { result.Bearer, result.Refresh });

            if (_options.UseCache)
            {
                await _cacheService.SetAsync(result.Bearer);
            }

            return result;
        }

        public virtual async Task<RefreshClaimsResult> RefreshClaimsAsync(string token)
        {
            var tokenModel = await _tokenStore.GetAsync(token);
            if (tokenModel == null || tokenModel.Type != TokenType.Bearer)
            {
                return RefreshClaimsResult.Failed("invalid token");
            }

            var claims = await GetUserClaimsAsync(tokenModel.UserId);

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
                await _cacheService.RemoveAsync(tokenModel.Value, reason);
            }

            //Remove from db
            await _tokenStore.RemoveAsync(token);
        }

        private async Task<List<Claim>> GetUserClaimsAsync(string userId)
        {
            var claims = await _profileService.GetUserClaimsAsync(userId);
            if (claims.All(a => a.Type != ApiTokenClaimTypes.Subject))
            {
                claims.Add(new Claim(ApiTokenClaimTypes.Subject, userId));
            }

            return claims;
        }
    }
}