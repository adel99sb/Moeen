using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Configurations;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Providers;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Api.Infrastructure.Data;
using Moeen.Api.infrastructure.Middleware;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
builder.Logging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(AppSettings.Instance.ConnectionString);

});


// Identity
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer("Bearer", options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = AppSettings.Instance.JwtSettings.Issuer,
        ValidAudience = AppSettings.Instance.JwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(AppSettings.Instance.JwtSettings.SecretKey)),
        RoleClaimType = ClaimTypes.Role,
        NameClaimType = ClaimTypes.NameIdentifier
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtDiagnostics");
            logger.LogInformation(
                "JWT message received. Path={Path}, HasAuthorizationHeader={HasAuthorizationHeader}",
                context.HttpContext.Request.Path,
                context.Request.Headers.ContainsKey("Authorization"));
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtDiagnostics");
            logger.LogWarning(
                context.Exception,
                "JWT authentication failed. Path={Path}, Error={Error}",
                context.HttpContext.Request.Path,
                context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtDiagnostics");
            logger.LogInformation(
                "JWT token validated. Path={Path}, UserId={UserId}, Roles={Roles}",
                context.HttpContext.Request.Path,
                context.Principal?.FindFirstValue("UserIdentifier") ?? context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier),
                string.Join(",", context.Principal?.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase)).Select(c => c.Value) ?? Array.Empty<string>()));
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            var logger = context.HttpContext.RequestServices.GetRequiredService<ILoggerFactory>()
                .CreateLogger("JwtDiagnostics");
            logger.LogWarning(
                "JWT challenge. Path={Path}, Error={Error}, Description={Description}, HasAuthorizationHeader={HasAuthorizationHeader}",
                context.HttpContext.Request.Path,
                context.Error,
                context.ErrorDescription,
                context.Request.Headers.ContainsKey("Authorization"));
            return Task.CompletedTask;
        }
    };
});

// Infrastructure - Repositories
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Providers
builder.Services.AddHttpContextAccessor();
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IFileService, FileService>();

// Application Services
builder.Services.AddScoped<IMosquService, MosquService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<IHalqaCommandService, HalqaCommandService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<IHalqaQueryService, HalqaQueryService>();
builder.Services.AddScoped<IHalqaTeacherAssignmentService , HalqaTeacherAssignmentService>();
builder.Services.AddScoped<IMemorizationService, MemorizationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<IExamCommandService, ExamCommandService>();
builder.Services.AddScoped<IExamQueryService, ExamQueryService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<ILessonManagementService, LessonManagementService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IContentSharingService, ContentSharingService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IExamHalqaService, ExamHalqaService>();
builder.Services.AddScoped<IBackupService, BackupService>();
builder.Services.AddScoped<IFoujService, FoujService>();
builder.Services.AddScoped<IStudentNotesService, StudentNotesService>();
builder.Services.AddScoped<IDailyAssignmentService, DailyAssignmentService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Moeen API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seeders
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await AppSeeder.SeedRolesAsync(roleManager);
}

if (app.Environment.IsDevelopment())
{
    await AppSeeder.SeedDevelopmentDataAsync(app.Services, app.Configuration);

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<ApiRequestLoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.Run();
