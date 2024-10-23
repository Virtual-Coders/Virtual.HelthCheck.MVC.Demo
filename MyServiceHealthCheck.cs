using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Virtual.HealthCheck
{
    public class MyServiceHealthCheck : IHealthCheck
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public MyServiceHealthCheck(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.GetAsync("https://dummyapi.online/api/movies");

                if (response.IsSuccessStatusCode)
                {
                    // Service is healthy
                    return HealthCheckResult.Healthy("The service is healthy.");
                }
                else
                {
                    // Service is reachable but responding with errors
                    return HealthCheckResult.Unhealthy("The service is unhealthy.");
                }
            }
            catch (HttpRequestException ex)
            {
                // Service is not reachable
                return HealthCheckResult.Unhealthy("The service is not reachable.", ex);
            }
        }
    }

}
