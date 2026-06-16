using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Moeen.Dashboard.Components.Shared;

public sealed class DashboardCrashBoundary : ErrorBoundary
{
    [Inject] private ILogger<DashboardCrashBoundary> Logger { get; set; } = default!;
    [Inject] private NavigationManager NavigationManager { get; set; } = default!;

    protected override Task OnErrorAsync(Exception exception)
    {
        Logger.LogError(exception, "Unhandled dashboard component issue. Uri={Uri}, Error={Error}", NavigationManager.Uri, exception.Message);
        return Task.CompletedTask;
    }
}
