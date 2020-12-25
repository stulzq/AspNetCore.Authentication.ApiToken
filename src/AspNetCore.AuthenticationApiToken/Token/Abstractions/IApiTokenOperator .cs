using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenOperator
    {
        Task<TokenCreateResult> CreateAsync(string userId);

        Task<TokenCreateResult> RefreshAsync(string refreshToken);

        Task<RefreshClaimsResult> RefreshClaimsAsync(string token);

        Task RemoveAsync(string token, string reason = null);
    }
}