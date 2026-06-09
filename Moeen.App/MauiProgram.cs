using Microsoft.Extensions.Logging;
using Moeen.App.Infrastructure.Http;
using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.App.Services.Implementations;

namespace Moeen.App
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddHttpClient();
            builder.Services.AddHttpClient<ContentShaeringApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });

            builder.Services.AddScoped<IContentShaeringService,
                ContentShaeringService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
