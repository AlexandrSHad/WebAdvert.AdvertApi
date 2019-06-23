using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using WebAdvert.AdvertApi.Services;

namespace WebAdvert.AdvertApi.HealthChecks
{
    public class StorageHealthCheck : IHealthCheck
    {
        private readonly IAdvertStorageService _advertStorageService;

        public StorageHealthCheck(IAdvertStorageService advertStorageService)
        {
            _advertStorageService = advertStorageService;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            var isStorageAlive = await _advertStorageService.CheckHealthAsync();

            return isStorageAlive ? HealthCheckResult.Healthy() : HealthCheckResult.Unhealthy();
        }
    }
}
