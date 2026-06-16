using System.Diagnostics;
using System.Security.Claims;

namespace Moeen.Api.infrastructure.Middleware;

public sealed class ApiRequestLoggingMiddleware
{
    private static readonly HashSet<string> IgnoredExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".css", ".js", ".map", ".png", ".jpg", ".jpeg", ".gif", ".svg", ".ico", ".woff", ".woff2"
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ApiRequestLoggingMiddleware> _logger;

    public ApiRequestLoggingMiddleware(RequestDelegate next, ILogger<ApiRequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (ShouldSkip(context.Request.Path))
        {
            await _next(context);
            return;
        }

        var correlationId = GetOrCreateCorrelationId(context);
        context.Response.Headers["X-Correlation-Id"] = correlationId;

        var stopwatch = Stopwatch.StartNew();
        var hasAuthHeader = context.Request.Headers.ContainsKey("Authorization");

        using var scope = _logger.BeginScope(new Dictionary<string, object?>
        {
            ["CorrelationId"] = correlationId,
            ["TraceIdentifier"] = context.TraceIdentifier,
            ["RequestPath"] = context.Request.Path.Value,
            ["RequestMethod"] = context.Request.Method
        });

        _logger.LogInformation(
            "API request started. Method={Method}, Path={Path}, Query={Query}, HasAuthHeader={HasAuthHeader}, RemoteIp={RemoteIp}",
            context.Request.Method,
            context.Request.Path,
            context.Request.QueryString.HasValue ? context.Request.QueryString.Value : string.Empty,
            hasAuthHeader,
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown");

        try
        {
            await _next(context);
            stopwatch.Stop();

            var statusCode = context.Response.StatusCode;
            var logLevel = statusCode >= 500 ? LogLevel.Error : statusCode >= 400 ? LogLevel.Warning : LogLevel.Information;

            _logger.Log(
                logLevel,
                "API request finished. Method={Method}, Path={Path}, StatusCode={StatusCode}, ElapsedMs={ElapsedMs}, UserId={UserId}, Roles={Roles}",
                context.Request.Method,
                context.Request.Path,
                statusCode,
                stopwatch.ElapsedMilliseconds,
                GetUserId(context.User) ?? "anonymous",
                GetRoles(context.User));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "API request crashed. Method={Method}, Path={Path}, ElapsedMs={ElapsedMs}, UserId={UserId}, Error={Error}",
                context.Request.Method,
                context.Request.Path,
                stopwatch.ElapsedMilliseconds,
                GetUserId(context.User) ?? "anonymous",
                ex.Message);
            throw;
        }
    }

    private static bool ShouldSkip(PathString path)
    {
        var value = path.Value;
        if (string.IsNullOrWhiteSpace(value))
            return false;

        var extension = Path.GetExtension(value);
        return !string.IsNullOrWhiteSpace(extension) && IgnoredExtensions.Contains(extension);
    }

    private static string GetOrCreateCorrelationId(HttpContext context)
    {
        const string headerName = "X-Correlation-Id";
        if (context.Request.Headers.TryGetValue(headerName, out var incoming) && !string.IsNullOrWhiteSpace(incoming.ToString()))
            return incoming.ToString();

        return context.TraceIdentifier;
    }

    private static string? GetUserId(ClaimsPrincipal user)
        => user.FindFirstValue("UserIdentifier")
           ?? user.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? user.FindFirstValue("sub");

    private static string GetRoles(ClaimsPrincipal user)
    {
        var roles = user.Claims
            .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase) || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase))
            .Select(c => c.Value)
            .Distinct(StringComparer.OrdinalIgnoreCase);

        var value = string.Join(",", roles);
        return string.IsNullOrWhiteSpace(value) ? "none" : value;
    }
}
