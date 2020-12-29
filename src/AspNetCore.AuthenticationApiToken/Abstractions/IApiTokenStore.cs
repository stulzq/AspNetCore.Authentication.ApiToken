using System.Collections.Generic;
using System.Threading.Tasks;

namespace AspNetCore.Authentication.ApiToken.Abstractions
{
    public interface IApiTokenStore
    {
        Task StoreAsync(TokenModel token);

        Task StoreAsync(List<TokenModel> token);

        Task<TokenModel> GetAsync(string token, string scheme);

        Task<List<TokenModel>> GetListAsync(string userId, string scheme);

        Task<List<TokenModel>> GetListAsync(string userId, string scheme, TokenType type);

        Task UpdateAsync(TokenModel token);

        Task UpdateListAsync(List<TokenModel> token);

        Task RemoveAsync(string token, string scheme);

        Task RemoveListAsync(string userId, string scheme);

        Task RemoveListAsync(string userId, string scheme, TokenType type);

        Task<int> RemoveExpirationAsync();
    }
}