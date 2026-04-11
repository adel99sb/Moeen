using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Moeen.Api.Application.Services;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Configurations;
using Moeen.Api.infrastructure.Data;
using Moeen.Api.infrastructure.Providers;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ≈÷«›… DbContext √Ê·«
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//  ”ÃÌ· «·„” Êœ⁄ «·⁄«„ (Generic)
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(option =>
    option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//  ”ÃÌ· UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// («Œ Ì«—Ì)  ”ÃÌ· «·„” Êœ⁄ «·⁄«„ ≈–« √—œ  «” Œœ«„Â „»«‘—…
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

//  ”ÃÌ· «·„” Êœ⁄«  «·„Œ’’… („À«·)
// builder.Services.AddScoped<IUserRepository, UserRepository>();


var app = builder.Build();
//  ”ÃÌ· «·„” Êœ⁄ «·⁄«„ (Generic Repository) ·Ì „ﬂ‰ «·‹ DI „‰ Õﬁ‰Â
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));

////////////////////////////////////
// —»ÿ ≈⁄œ«œ«  JwtSettings „‰ „·› appsettings.json
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>();
builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

// ≈÷«›… Identity
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

// ≈÷«›… „’«œﬁ… JWT
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

            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,

            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.Secret)),

            RoleClaimType = ClaimTypes.Role
        };
    });
// («Œ Ì«—Ì)  ”ÃÌ· JwtService ≈–« ﬂ‰  ” ” Œœ„Â ›Ì AuthController
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IMosquService, MosquService>();
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
builder.Services.AddScoped<ICircleCommandService, CircleCommandService>();
builder.Services.AddScoped<IRegistrationService, RegistrationService>();
builder.Services.AddScoped<ICircleQueryService, CircleQueryService>();
builder.Services.AddScoped<ICircleTeacherAssignmentService, CircleTeacherAssignmentService>();
builder.Services.AddScoped<IQuranCurriculumService, QuranCurriculumService>();
builder.Services.AddScoped<IMemorizationService, MemorizationService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IPointsService, PointsService>();
builder.Services.AddScoped<IExamGradingCriteriaService, ExamGradingCriteriaService>();
builder.Services.AddScoped<IExamPhaseService, ExamPhaseService>();
builder.Services.AddScoped<IExamCommandService, ExamCommandService>();
builder.Services.AddScoped<IExamQueryService, ExamQueryService>();
builder.Services.AddScoped<IGoalService, GoalService>();
builder.Services.AddScoped<IImportExportService, ImportExportService>();
builder.Services.AddScoped<ILessonManagementService, LessonManagementService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IContentSharingService, ContentSharingService>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
builder.Services.AddScoped<ILibraryService, LibraryService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<ISystemConfigurationService, SystemConfigurationService>();
builder.Services.AddScoped<ISchedulingService, SchedulingService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<FileStorageSettings>(builder.Configuration.GetSection("FileStorageSettings"));
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IVerificationService, VerificationService>();
builder.Services.AddScoped<IFileService, FileService>();
//////////////////////////////////////
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Moeen API",
        Version = "v1"
    });

    // JWT Auth definition
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your token}"
    });

    // Apply JWT globally
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
            new string[] {}
        }
    });
});
var app = builder.Build();
//seeders
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    await AppSeeder.SeedRolesAsync(roleManager);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
