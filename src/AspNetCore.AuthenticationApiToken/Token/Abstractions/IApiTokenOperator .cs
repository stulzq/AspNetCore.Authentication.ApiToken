using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenOperator
    {
        Task<ApiTokenCreateResult> CreateAsync(string userId);

        Task<ApiTokenCreateResult> RefreshAsync(string refreshToken);

        Task<RefreshClaimsResult> RefreshClaimsAsync(string token);

        Task RemoveAsync(string token, string reason = null);
    }
}