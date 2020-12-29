using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenOperator
    {
        Task<TokenCreateResult> CreateAsync(string userId, string scheme = null);

        Task<TokenCreateResult> RefreshAsync(string refreshToken, string scheme = null);

        Task<RefreshClaimsResult> RefreshClaimsAsync(string token, string scheme = null);

        Task RemoveAsync(string token, string scheme = null);
    }
}