using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace AspNetCore.Authentication.ReferenceToken.Abstractions
{
    public class DefaultTokenOperator : ITokenOperator
    {
        private readonly IOptionsMonitor<ReferenceTokenOptions> _options;
        private readonly IProfileService _profileService;
        private readonly ITokenStore _tokenStore;
        private readonly ITokenCacheService _cacheService;

        public DefaultTokenOperator(IOptionsMonitor<ReferenceTokenOptions> options,
            IProfileService profileService,
            ITokenStore tokenStore,
            ITokenCacheService cacheService)
        {
            _options = options;
            _profileService = profileService;
            _tokenStore = tokenStore;
            _cacheService = cacheService;
        }

        public virtual async Task<TokenCreateResult> CreateAsync(string userId)
        {
            var claims = await _profileService.GetUserClaimsAsync(userId);
            var result = CreateByUserClaims(userId, claims);

            if (!_options.CurrentValue.AllowMultiTokenActiveForOneUser)
            {
                const string reason = "Only one token active at the same time is allowed.";

                //Remove old token from cache
                if (_options.CurrentValue.UseCache)
                {
                    var tokenList = await _tokenStore.GetListAsync(userId);
                    foreach (var token in tokenList)
                    {
                        await _cacheService.RemoveAsync(token, reason);
                    }
                }

                //Remove old token from db
                await _tokenStore.RemoveListAsync(userId, reason);
            }

            await _tokenStore.StoreAsync(new List<TokenModel>() {result.Token, result.Refresh});

            if (_options.CurrentValue.UseCache)
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
                Expiration = now + _options.CurrentValue.TokenExpire
            };

            var refreshToken = new TokenModel()
            {
                Token = TokenTools.CreateToken(userId),
                CreateTime = now,
                Type = TokenType.Refresh,
                UserId = userId,
                Claims = null,
                Expiration = now + _options.CurrentValue.RefreshTokenExpire
            };

            return TokenCreateResult.Success(token, refreshToken);
        }

        public virtual async Task<TokenCreateResult> RefreshAsync(string refreshToken)
        {
            var token = await _tokenStore.GetAsync(refreshToken);
            if (token == null || token.Type != TokenType.Refresh)
            {
                return TokenCreateResult.Failed("invalid refresh_token");
            }

            if (token.Expiration.UtcDateTime < DateTimeOffset.UtcNow)
            {
                return TokenCreateResult.Failed($"The refresh_token expired at '{token.Expiration.LocalDateTime.ToString(CultureInfo.InvariantCulture)}'");
            }

            var claims = await _profileService.GetUserClaimsAsync(token.UserId);
            var result = CreateByUserClaims(token.UserId, claims);

            await _tokenStore.StoreAsync(new List<TokenModel>() { result.Token, result.Refresh });

            if (_options.CurrentValue.UseCache)
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

            if (tokenModel.Expiration.UtcDateTime < DateTimeOffset.UtcNow)
            {
                return RefreshClaimsResult.Failed($"The token expired at '{tokenModel.Expiration.LocalDateTime.ToString(CultureInfo.InvariantCulture)}'");
            }

            var claims = await _profileService.GetUserClaimsAsync(tokenModel.UserId);

            //Refresh db
            await _tokenStore.UpdateClaimsAsync(token, claims);

            //Refresh cache
            if (_options.CurrentValue.UseCache)
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
            if (_options.CurrentValue.UseCache)
            {
                await _cacheService.RemoveAsync(tokenModel, reason);
            }

            //Remove from db
            await _tokenStore.RemoveAsync(token, reason);
        }
    }
}