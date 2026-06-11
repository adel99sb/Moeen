using Microsoft.Extensions.Logging;
using Moeen.App.Infrastructure.Http;
using Moeen.App.Infrastructure.Http.Clients;
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

            builder.Services.AddHttpClient<DailyAssignmentsApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<BooksApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<UserApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<PointsApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<AttendanceApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<EnrollmentApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });
            builder.Services.AddHttpClient<FeedbackApiClient>(client =>
            {
                client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
            });

            builder.Services.AddScoped<IContentShaeringService,
                ContentShaeringService>();

            builder.Services.AddScoped<IDailyAssignmentsService,
                DailyAssignmentsService>();

            builder.Services.AddScoped<ILibraryService, LibraryService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPointsService, PointsService>();
            builder.Services.AddScoped<IAttendanceService, AttendanceService>();
            builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
            builder.Services.AddScoped<IFeedbackService, FeedbackService>();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
