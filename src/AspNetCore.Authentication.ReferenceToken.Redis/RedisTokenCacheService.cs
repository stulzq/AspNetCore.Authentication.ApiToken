using System;
using System.Threading.Tasks;
using AspNetCore.Authentication.ReferenceToken.Abstractions;

namespace AspNetCore.Authentication.ReferenceToken.Redis
{
    public class RedisTokenCacheService : ITokenCacheService
    {
        public Task<TokenCacheModel> GetAsync(string token)
        {
            throw new NotImplementedException();
        }

        public Task SetAsync(TokenModel token)
        {
            //判断过期 过期时间+缓存穿透预防时间>0 设置缓存 否则 设置缓存穿透

            throw new NotImplementedException();
        }

        public Task SetNullAsync(string invalidToken)
        {
            //缓存穿透
            throw new NotImplementedException();
        }

        public Task RemoveAsync(TokenModel token, string reason = null)
        {
            //判断reason是否为null 不为null 设置缓存用于显示消息
            throw new NotImplementedException();
        }
    }
}
