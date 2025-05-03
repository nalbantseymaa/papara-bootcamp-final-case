namespace ExpenseTracking.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate next;
    private readonly ILogger<RequestLoggingMiddleware> logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        this.next = next;
        this.logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        var request = context.Request;

        var method = request.Method;
        var path = request.Path;
        var queryString = request.QueryString.HasValue ? request.QueryString.Value : "";
        var ip = context.Connection.RemoteIpAddress?.ToString();

        logger.LogInformation("📥 HTTP Request: {Method} {Path}{QueryString} from {IP}",
            method, path, queryString, ip);

        await next(context);

        var statusCode = context.Response.StatusCode;
        logger.LogInformation("📤 HTTP Response: {StatusCode} for {Method} {Path}", statusCode, method, path);
    }
}
