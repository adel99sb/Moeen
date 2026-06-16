using System.Diagnostics;
using System.Net;

namespace Moeen.Dashboard.Infrastructure.Http.Handlers;

public sealed class ApiHttpDiagnosticsHandler : DelegatingHandler
{
    private readonly ILogger<ApiHttpDiagnosticsHandler> _logger;

    public ApiHttpDiagnosticsHandler(ILogger<ApiHttpDiagnosticsHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();
        _logger.LogInformation("Dashboard API call started. Method={Method}, Url={Url}", request.Method, request.RequestUri);

        try
        {
            var response = await base.SendAsync(request, cancellationToken);
            stopwatch.Stop();
            var level = response.StatusCode >= HttpStatusCode.InternalServerError ? LogLevel.Error : response.StatusCode >= HttpStatusCode.BadRequest ? LogLevel.Warning : LogLevel.Information;
            _logger.Log(level, "Dashboard API call finished. Method={Method}, Url={Url}, StatusCode={StatusCode}, ElapsedMs={ElapsedMs}", request.Method, request.RequestUri, (int)response.StatusCode, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Dashboard API call crashed. Method={Method}, Url={Url}, ElapsedMs={ElapsedMs}, Error={Error}", request.Method, request.RequestUri, stopwatch.ElapsedMilliseconds, ex.Message);
            throw;
        }
    }
}
