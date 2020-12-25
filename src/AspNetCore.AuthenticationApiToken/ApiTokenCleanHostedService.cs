using System;
using System.Threading;
using System.Threading.Tasks;
using AspNetCore.Authentication.ApiToken.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Timer = System.Threading.Timer;

namespace AspNetCore.Authentication.ApiToken
{
    public class ApiTokenCleanHostedService : IHostedService
    {
        private readonly ILogger<ApiTokenCleanHostedService> _logger;
        private readonly IApiTokenCacheService _cache;
        private readonly IServiceProvider _serviceProvider;
        private readonly ApiTokenCleanOptions _options;

        private const string LockKey = "cleanlock";

        private Timer _timer;

        public ApiTokenCleanHostedService(IOptions<ApiTokenCleanOptions> options,
            ILogger<ApiTokenCleanHostedService> logger,
            IApiTokenCacheService cache,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _cache = cache;
            _serviceProvider = serviceProvider;
            _options = options.Value;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            if (_options.Interval <= 0)
            {
                _logger.LogInformation($"The value of Interval is {_options.Interval}, service will not start.");
                return Task.CompletedTask;
            }

            _timer = new Timer(async _ => await DoWorkAsync(), null, TimeSpan.Zero,
                TimeSpan.FromSeconds(_options.Interval));
            _logger.LogInformation("Start success.");
            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            var lockValue = Guid.NewGuid().ToString();

            //Request lock
            if (!await _cache.LockTakeAsync(LockKey, lockValue, TimeSpan.FromSeconds(60)))
            {
                _logger.LogInformation("Request lock failed, not run.");
                return;
            }

            using var scope = _serviceProvider.CreateScope();
            var tokenStore = scope.ServiceProvider.GetRequiredService<IApiTokenStore>();
            var cleanCount = await tokenStore.RemoveExpirationAsync();

            _logger.LogInformation($"{cleanCount} expired token records have been deleted.");
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _logger.LogInformation("Stop success.");
            return Task.CompletedTask;
        }
    }
}