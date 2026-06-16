namespace Moeen.Dashboard.Infrastructure.Middleware;

public sealed class DashboardRequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<DashboardRequestLoggingMiddleware> _logger;

    public DashboardRequestLoggingMiddleware(RequestDelegate next, ILogger<DashboardRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        _logger.LogInformation("Dashboard request started. Method={Method}, Path={Path}", context.Request.Method, context.Request.Path);
        await _next(context);
        _logger.LogInformation("Dashboard request finished. Method={Method}, Path={Path}, StatusCode={StatusCode}", context.Request.Method, context.Request.Path, context.Response.StatusCode);
    }
}
