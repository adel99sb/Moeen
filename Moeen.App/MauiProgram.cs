using Microsoft.Extensions.Logging;
using Moeen.App.Infrastructure.Http;
using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.App.Services;
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
            builder.Services.AddHttpClient<AuthApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<StudentDashboardApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<StudentProgressApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<StudentProfileApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<StudentBookApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<StudentPostApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<ParentDashboardApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<ParentProgressApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<ParentProfileApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<ParentLibraryApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<ParentPostApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });

            builder.Services.AddScoped<AppSessionService>();
            builder.Services.AddScoped<IAppAuthService, AppAuthService>();
            builder.Services.AddScoped<IStudentDashboardService, StudentDashboardService>();
            builder.Services.AddScoped<IStudentProgressService, StudentProgressService>();
            builder.Services.AddScoped<IStudentProfileService, StudentProfileService>();
            builder.Services.AddScoped<IStudentBookService, StudentBookService>();
            builder.Services.AddScoped<IStudentPostService, StudentPostService>();
            builder.Services.AddScoped<IParentDashboardService, ParentDashboardService>();
            builder.Services.AddScoped<IParentProgressService, ParentProgressService>();
            builder.Services.AddScoped<IParentProfileService, ParentProfileService>();
            builder.Services.AddScoped<IParentLibraryService, ParentLibraryService>();
            builder.Services.AddScoped<IParentPostService, ParentPostService>();
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
