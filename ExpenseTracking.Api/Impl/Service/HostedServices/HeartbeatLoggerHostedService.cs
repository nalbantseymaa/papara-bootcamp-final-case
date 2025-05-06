using Microsoft.Extensions.Options;

namespace ExpenseTracking.Api.Middleware;

public class HeartbeatLoggerHostedService : BackgroundService
{
    private readonly ILogger<HeartbeatLoggerHostedService> logger;
    private readonly HeartbeatOptions options;

    public HeartbeatLoggerHostedService(
        ILogger<HeartbeatLoggerHostedService> logger,
        IOptions<HeartbeatOptions> options)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        ValidateOptions(this.options);
    }

    private void ValidateOptions(HeartbeatOptions opts)
    {
        if (string.IsNullOrWhiteSpace(opts.ApplicationName))
            throw new ArgumentException("Missing 'ApplicationName' in HeartbeatOptions");

        if (opts.Interval <= TimeSpan.Zero)
            throw new ArgumentException("Missing or invalid 'Interval' in HeartbeatOptions");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("HeartbeatLoggerHostedService started for: {App}", options.ApplicationName);

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("💓 Heartbeat from {ApplicationName} at {Time}", options.ApplicationName, DateTime.UtcNow);
            try
            {
                await Task.Delay(options.Interval, stoppingToken);
            }
            catch (TaskCanceledException)
            {

            }
        }

        logger.LogInformation("HeartbeatLoggerHostedService stopped for: {App}", options.ApplicationName);
    }
}

public class HeartbeatOptions
{
    public string ApplicationName { get; set; } = string.Empty;
    public TimeSpan Interval { get; set; } = TimeSpan.FromMinutes(1);
}