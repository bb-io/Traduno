using Polly;
using Polly.Retry;
using RestSharp;
using System.Globalization;
using System.Net;

namespace Apps.Traduno.Api;

public static class TradunoPollyPolicies
{
    public static ResiliencePipeline<RestResponse> GetTooManyRequestsRetryPolicy(int retryCount = 6)
    {
        var retryOptions = new RetryStrategyOptions<RestResponse>
        {
            MaxRetryAttempts = retryCount,
            ShouldHandle = new PredicateBuilder<RestResponse>()
                .HandleResult(r =>
                    r.StatusCode == HttpStatusCode.TooManyRequests ||
                    r.StatusCode == HttpStatusCode.InternalServerError ||
                    r.StatusCode == HttpStatusCode.BadGateway ||
                    r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                    r.StatusCode == HttpStatusCode.GatewayTimeout),
            DelayGenerator = args =>
            {
                var retryAfter = args.Outcome.Result?.Headers?
                    .FirstOrDefault(h => h.Name?.Equals("Retry-After", StringComparison.OrdinalIgnoreCase) == true)
                    ?.Value?.ToString();

                if (TryParseRetryAfter(retryAfter, out var delay))
                {
                    return new ValueTask<TimeSpan?>(delay);
                }

                var fallbackDelay = TimeSpan.FromSeconds(Random.Shared.Next(5, 16));
                return new ValueTask<TimeSpan?>(fallbackDelay);
            }
        };

        return new ResiliencePipelineBuilder<RestResponse>()
            .AddRetry(retryOptions)
            .Build();
    }

    private static bool TryParseRetryAfter(string? value, out TimeSpan delay)
    {
        if (int.TryParse(value, out var seconds))
        {
            delay = TimeSpan.FromSeconds(seconds);
            return true;
        }

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal,
                out var retryAt))
        {
            delay = retryAt - DateTimeOffset.UtcNow;
            if (delay < TimeSpan.Zero)
            {
                delay = TimeSpan.Zero;
            }

            return true;
        }

        delay = default;
        return false;
    }
}
