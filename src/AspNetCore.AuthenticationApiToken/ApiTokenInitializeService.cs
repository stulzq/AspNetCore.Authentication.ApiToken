using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenInitializeService: IHostedService
    {
        private readonly IApiTokenCacheService _cacheService;
        private readonly ILogger<ApiTokenInitializeService> _logger;

        public ApiTokenInitializeService(IApiTokenCacheService cacheService,ILogger<ApiTokenInitializeService> logger)
        {
            _cacheService = cacheService;
            _logger = logger;
        }
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _cacheService.InitializeAsync();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}