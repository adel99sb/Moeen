using Microsoft.Extensions.Http;

namespace Moeen.Dashboard.Infrastructure.Http.Handlers;

public sealed class ApiHttpDiagnosticsFilter : IHttpMessageHandlerBuilderFilter
{
    public Action<HttpMessageHandlerBuilder> Configure(Action<HttpMessageHandlerBuilder> next)
    {
        return builder =>
        {
            next(builder);
            builder.AdditionalHandlers.Insert(0, builder.Services.GetRequiredService<ApiHttpDiagnosticsHandler>());
        };
    }
}
