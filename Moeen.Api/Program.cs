using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
using Moeen.Shared.Responses;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.TeacherDashboard;
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
builder.Services.AddScoped<IStudentMobileDashboardService, StudentMobileDashboardService>();
builder.Services.AddScoped<IStudentMobileProgressService, StudentMobileProgressService>();
builder.Services.AddScoped<IStudentMobileProfileService, StudentMobileProfileService>();
builder.Services.AddScoped<IStudentMobilePostService, StudentMobilePostService>();
builder.Services.AddScoped<IParentMobileDashboardService, ParentMobileDashboardService>();
builder.Services.AddScoped<IParentMobileProgressService, ParentMobileProgressService>();
builder.Services.AddScoped<IParentMobileProfileService, ParentMobileProfileService>();
builder.Services.AddScoped<IParentMobilePostService, ParentMobilePostService>();
builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.InvalidModelStateResponseFactory = context =>
        {
            var errors = context.ModelState
                .Where(item => item.Value?.Errors.Count > 0)
                .ToDictionary(
                    item => item.Key,
                    item => item.Value!.Errors
                        .Select(error => string.IsNullOrWhiteSpace(error.ErrorMessage) ? "Invalid value." : error.ErrorMessage)
                        .ToArray());

            return new ObjectResult(GeneralResponse.BadRequest("Invalid request data.", errors))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        };
    });
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

await InitializeDatabaseAsync(app);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<ApiRequestLoggingMiddleware>();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "Moeen.Api",
    Environment = app.Environment.EnvironmentName,
    UtcTime = DateTimeOffset.UtcNow
}));
app.MapGet("/api/health", () => Results.Ok(new
{
    Status = "Healthy",
    Service = "Moeen.Api",
    Environment = app.Environment.EnvironmentName,
    UtcTime = DateTimeOffset.UtcNow
}));
app.MapGet("/api/teacher-dashboard/overview", GetCurrentTeacherOverviewAsync).RequireAuthorization();
app.MapGet("/api/teacher-dashboard/my-halaqas-progress", GetCurrentTeacherHalaqasProgressAsync).RequireAuthorization();

app.Run();

static async Task InitializeDatabaseAsync(WebApplication app)
{
    var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup.Database");

    try
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        logger.LogInformation(
            "Database startup initialization. Environment={Environment}, Provider={Provider}, Connection={Connection}",
            app.Environment.EnvironmentName,
            context.Database.ProviderName,
            MaskConnectionString(context.Database.GetDbConnection().ConnectionString));

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Applying EF Core migrations for local development startup...");
            await context.Database.MigrateAsync();
            logger.LogInformation("EF Core migrations are up to date.");
        }
        else
        {
            logger.LogInformation("Skipping automatic migrations because the environment is not Development.");
        }

        logger.LogInformation("Ensuring application roles exist...");
        await AppSeeder.SeedRolesAsync(roleManager);
        logger.LogInformation("Application roles are ready.");

        if (app.Environment.IsDevelopment())
        {
            logger.LogInformation("Seeding development data if missing...");
            await AppSeeder.SeedDevelopmentDataAsync(app.Services, app.Configuration);
            await LocalDevData.ApplyAsync(app.Services);
            logger.LogInformation("Development seed data is ready.");
        }
    }
    catch (Exception ex)
    {
        logger.LogCritical(
            ex,
            "Moeen API startup database initialization failed. Check SQL Server/LocalDB is installed and running, the connection string is correct, and the database user has permission to create/update the database.");
        throw;
    }
}

static string MaskConnectionString(string? connectionString)
{
    if (string.IsNullOrWhiteSpace(connectionString))
        return "<empty>";

    var parts = connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
        .Select(part =>
        {
            var separatorIndex = part.IndexOf('=');
            if (separatorIndex <= 0)
                return part;

            var key = part[..separatorIndex].Trim();
            var value = part[(separatorIndex + 1)..].Trim();
            var normalizedKey = key.Replace(" ", string.Empty, StringComparison.OrdinalIgnoreCase).ToLowerInvariant();

            if (normalizedKey is "password" or "pwd" or "user id" or "userid" or "uid")
                return $"{key}=***";

            return $"{key}={value}";
        });

    return string.Join(';', parts);
}

static async Task<IResult> GetCurrentTeacherOverviewAsync(ClaimsPrincipal user, AppDbContext context)
{
    if (!user.IsInRole("Teacher"))
        return Results.Forbid();

    var teacherId = GetCurrentTeacherId(user);
    if (teacherId is null)
        return Results.Unauthorized();

    var teacher = await context.Teachers.AsNoTracking()
        .Where(t => t.Id == teacherId.Value)
        .Select(t => new { t.Id, Name = t.name ?? string.Empty, MosqueName = t.Mosque != null ? t.Mosque.name : string.Empty })
        .FirstOrDefaultAsync();

    if (teacher is null)
    {
        teacher = await context.Users.AsNoTracking()
            .Where(u => u.Id == teacherId.Value)
            .Select(u => new { u.Id, Name = u.name ?? string.Empty, MosqueName = string.Empty })
            .FirstOrDefaultAsync();

        if (teacher is null)
            return Results.NotFound();
    }

    var regularHalaqas = await context.Halqas.AsNoTracking()
        .Where(h => h.TeacherId == teacherId.Value)
        .OrderBy(h => h.Name)
        .Select(h => new { h.Id, Name = h.Name ?? string.Empty, Type = h.Type ?? string.Empty })
        .ToListAsync();

    var saturdayHalaqas = await context.SaturdayHalqes.AsNoTracking()
        .Where(h => h.TeacherId == teacherId.Value)
        .OrderBy(h => h.name)
        .Select(h => new { h.Id, Name = h.name ?? string.Empty, Type = "حلقة أسبوعية" })
        .ToListAsync();

    var halaqas = regularHalaqas
        .Concat(saturdayHalaqas)
        .GroupBy(h => h.Id)
        .Select(g => g.First())
        .OrderBy(h => h.Name)
        .ToList();

    var halaqaIds = halaqas.Select(h => h.Id).ToList();
    var studentRows = halaqaIds.Count == 0
        ? new List<TeacherDashboardStudentLookup>()
        : (await context.Students.AsNoTracking()
            .Where(s =>
                (s.HalqaId.HasValue && halaqaIds.Contains(s.HalqaId.Value)) ||
                (s.SaturdayHalqaId.HasValue && halaqaIds.Contains(s.SaturdayHalqaId.Value)) ||
                (s.SaturdayHalqeId != Guid.Empty && halaqaIds.Contains(s.SaturdayHalqeId)))
            .Select(s => new
            {
                s.Id,
                Name = s.name ?? string.Empty,
                s.HalqaId,
                s.SaturdayHalqaId,
                s.SaturdayHalqeId,
                Score = s.score
            })
            .ToListAsync())
            .Select(s => new TeacherDashboardStudentLookup(s.Id, s.Name, s.HalqaId, s.SaturdayHalqaId, s.SaturdayHalqeId, s.Score))
            .ToList();

    var students = studentRows
        .Select(s =>
        {
            Guid? circleId = s.HalqaId.HasValue && halaqaIds.Contains(s.HalqaId.Value)
                ? s.HalqaId
                : s.SaturdayHalqaId.HasValue && halaqaIds.Contains(s.SaturdayHalqaId.Value)
                    ? s.SaturdayHalqaId
                    : s.SaturdayHalqeId != Guid.Empty && halaqaIds.Contains(s.SaturdayHalqeId)
                        ? s.SaturdayHalqeId
                        : null;

            return new TeacherDashboardStudentRow(s.Id, s.Name, circleId, s.Score);
        })
        .Where(s => s.HalqaId.HasValue)
        .GroupBy(s => s.Id)
        .Select(g => g.First())
        .ToList();

    var progress = await LoadTeacherProgressRowsAsync(context, teacherId.Value, halaqaIds);
    var attendance = await LoadTeacherAttendanceRowsAsync(context, teacherId.Value, students.Select(s => s.Id).ToList());
    var latest = GetLatestTeacherProgressByStudent(progress);
    var today = DateTime.UtcNow.Date;
    var monthStart = new DateTime(today.Year, today.Month, 1);

    string HalaqaName(Guid? halaqaId) => halaqas.FirstOrDefault(h => h.Id == halaqaId)?.Name ?? string.Empty;

    var response = new TeacherDashboardOverviewResponse
    {
        TeacherId = teacher.Id,
        TeacherName = teacher.Name,
        MosqueName = teacher.MosqueName,
        TotalHalaqas = halaqas.Count,
        TotalStudents = students.Count,
        AttendanceTodayPercent = TeacherAttendanceRate(attendance.Where(a => a.Date.Date == today).Select(a => a.Status)),
        TotalMemorizedPages = latest.Values.Sum(p => Math.Max(0, p.MemorizedUntil)),
        ExcellencePoints = students.Sum(s => Math.Max(0, s.Score)),
        Halaqas = halaqas.Select(h =>
        {
            var halaqaStudents = students.Where(s => s.HalqaId == h.Id).ToList();
            var latestScores = halaqaStudents
                .Select(s => latest.TryGetValue(s.Id, out var row) ? row.LevelScore : 0)
                .Where(score => score > 0)
                .ToList();

            return new TeacherDashboardHalaqaMetricDto
            {
                HalqaId = h.Id,
                HalqaName = h.Name,
                HalqaType = h.Type,
                StudentsCount = halaqaStudents.Count,
                ProgressEntriesThisMonth = progress.Count(p => p.HalqaId == h.Id && p.Date >= monthStart),
                AttendanceRatePercent = TeacherAttendanceRate(attendance.Where(a => a.HalqaId == h.Id && a.Date >= monthStart).Select(a => a.Status)),
                AverageLevelScore = latestScores.Count == 0 ? 0 : (int)Math.Round(latestScores.Average())
            };
        }).ToList()
    };

    response.TopStudents = latest.Values
        .Join(students, p => p.StudentId, s => s.Id, (p, s) => new { Progress = p, Student = s })
        .OrderByDescending(x => x.Progress.LevelScore)
        .ThenByDescending(x => x.Progress.MemorizedUntil)
        .Take(5)
        .Select(x => new TeacherDashboardStudentProgressDto
        {
            StudentId = x.Student.Id,
            StudentName = x.Student.Name,
            HalqaName = HalaqaName(x.Student.HalqaId),
            MemorizedUntil = x.Progress.MemorizedUntil,
            LatestPageNumber = x.Progress.PageNumber,
            LevelScore = x.Progress.LevelScore,
            Points = x.Student.Score,
            LastProgressDate = x.Progress.Date
        })
        .ToList();

    response.FollowUpStudents = BuildTeacherFollowUpStudents(students, HalaqaName, progress, attendance);
    response.FollowUpAlerts = response.FollowUpStudents.Count;

    return Results.Ok(response);
}

static async Task<IResult> GetCurrentTeacherHalaqasProgressAsync(ClaimsPrincipal user, AppDbContext context)
{
    if (!user.IsInRole("Teacher"))
        return Results.Forbid();

    var teacherId = GetCurrentTeacherId(user);
    if (teacherId is null)
        return Results.Unauthorized();

    var teacher = await context.Teachers.AsNoTracking()
        .Where(t => t.Id == teacherId.Value)
        .Select(t => new { t.Id, Name = t.name ?? string.Empty })
        .FirstOrDefaultAsync();

    if (teacher is null)
    {
        teacher = await context.Users.AsNoTracking()
            .Where(u => u.Id == teacherId.Value)
            .Select(u => new { u.Id, Name = u.name ?? string.Empty })
            .FirstOrDefaultAsync();

        if (teacher is null)
            return Results.NotFound();
    }

    var regularHalaqas = await context.Halqas.AsNoTracking()
        .Where(h => h.TeacherId == teacherId.Value)
        .OrderBy(h => h.Name)
        .Select(h => new { h.Id, Name = h.Name ?? string.Empty, Type = h.Type ?? string.Empty })
        .ToListAsync();

    var saturdayHalaqas = await context.SaturdayHalqes.AsNoTracking()
        .Where(h => h.TeacherId == teacherId.Value)
        .OrderBy(h => h.name)
        .Select(h => new { h.Id, Name = h.name ?? string.Empty, Type = "حلقة أسبوعية" })
        .ToListAsync();

    var halaqas = regularHalaqas
        .Concat(saturdayHalaqas)
        .GroupBy(h => h.Id)
        .Select(g => g.First())
        .OrderBy(h => h.Name)
        .ToList();

    var halaqaIds = halaqas.Select(h => h.Id).ToList();
    var studentRows = halaqaIds.Count == 0
        ? new List<TeacherDashboardStudentLookup>()
        : (await context.Students.AsNoTracking()
            .Where(s =>
                (s.HalqaId.HasValue && halaqaIds.Contains(s.HalqaId.Value)) ||
                (s.SaturdayHalqaId.HasValue && halaqaIds.Contains(s.SaturdayHalqaId.Value)) ||
                (s.SaturdayHalqeId != Guid.Empty && halaqaIds.Contains(s.SaturdayHalqeId)))
            .Select(s => new
            {
                s.Id,
                Name = s.name ?? string.Empty,
                s.HalqaId,
                s.SaturdayHalqaId,
                s.SaturdayHalqeId,
                Score = s.score
            })
            .ToListAsync())
            .Select(s => new TeacherDashboardStudentLookup(s.Id, s.Name, s.HalqaId, s.SaturdayHalqaId, s.SaturdayHalqeId, s.Score))
            .ToList();

    var students = studentRows
        .Select(s =>
        {
            Guid? circleId = s.HalqaId.HasValue && halaqaIds.Contains(s.HalqaId.Value)
                ? s.HalqaId
                : s.SaturdayHalqaId.HasValue && halaqaIds.Contains(s.SaturdayHalqaId.Value)
                    ? s.SaturdayHalqaId
                    : s.SaturdayHalqeId != Guid.Empty && halaqaIds.Contains(s.SaturdayHalqeId)
                        ? s.SaturdayHalqeId
                        : null;

            return new TeacherDashboardStudentRow(s.Id, s.Name, circleId, s.Score);
        })
        .Where(s => s.HalqaId.HasValue)
        .GroupBy(s => s.Id)
        .Select(g => g.First())
        .ToList();

    var progress = await LoadTeacherProgressRowsAsync(context, teacherId.Value, halaqaIds);
    var attendance = await LoadTeacherAttendanceRowsAsync(context, teacherId.Value, students.Select(s => s.Id).ToList());
    var latest = GetLatestTeacherProgressByStudent(progress);

    var response = new TeacherHalaqaProgressResponse
    {
        TeacherId = teacher.Id,
        TeacherName = teacher.Name,
        TotalHalaqas = halaqas.Count,
        TotalStudents = students.Count,
        TotalProgressEntries = progress.Count,
        AverageAttendanceRatePercent = TeacherAttendanceRate(attendance.Select(a => a.Status))
    };

    response.Halaqas = halaqas.Select(h =>
    {
        var halaqaStudents = students.Where(s => s.HalqaId == h.Id).ToList();
        var halaqaProgress = progress.Where(p => p.HalqaId == h.Id).ToList();
        var halaqaAttendance = attendance.Where(a => a.HalqaId == h.Id).ToList();
        var studentRows = halaqaStudents.Select(s =>
        {
            latest.TryGetValue(s.Id, out var last);
            var memorizedUntil = last?.MemorizedUntil ?? 0;

            return new TeacherStudentProgressDto
            {
                StudentId = s.Id,
                StudentName = s.Name,
                Points = s.Score,
                TotalEntries = halaqaProgress.Count(p => p.StudentId == s.Id),
                LatestJuzNumber = last?.JuzNumber ?? 0,
                LatestPageNumber = last?.PageNumber ?? 0,
                MemorizedUntil = memorizedUntil,
                NextTarget = last?.NextTarget ?? 0,
                LevelScore = last?.LevelScore ?? 0,
                AttendanceRatePercent = TeacherAttendanceRate(halaqaAttendance.Where(a => a.StudentId == s.Id).Select(a => a.Status)),
                MemorizationPercent = TeacherMemorizationPercent(memorizedUntil),
                LastProgressDate = last?.Date
            };
        }).OrderByDescending(s => s.LastProgressDate ?? DateTime.MinValue).ToList();

        return new TeacherHalaqaProgressDto
        {
            HalqaId = h.Id,
            HalqaName = h.Name,
            HalqaType = h.Type,
            StudentsCount = halaqaStudents.Count,
            ProgressEntriesCount = halaqaProgress.Count,
            AttendanceRatePercent = TeacherAttendanceRate(halaqaAttendance.Select(a => a.Status)),
            AverageLevelScore = studentRows.Count == 0 ? 0 : (int)Math.Round(studentRows.Average(s => s.LevelScore)),
            AverageMemorizationPercent = studentRows.Count == 0 ? 0 : (int)Math.Round(studentRows.Average(s => s.MemorizationPercent)),
            LastProgressDate = halaqaProgress.OrderByDescending(p => p.Date).Select(p => (DateTime?)p.Date).FirstOrDefault(),
            Students = studentRows
        };
    }).ToList();

    return Results.Ok(response);
}

static Guid? GetCurrentTeacherId(ClaimsPrincipal user)
{
    var value = user.FindFirstValue("UserIdentifier") ?? user.FindFirstValue(ClaimTypes.NameIdentifier);
    return Guid.TryParse(value, out var id) ? id : null;
}

static async Task<List<TeacherDashboardProgressRow>> LoadTeacherProgressRowsAsync(AppDbContext context, Guid teacherId, List<Guid> halaqaIds)
{
    return halaqaIds.Count == 0
        ? new List<TeacherDashboardProgressRow>()
        : await context.ProgressEntries.AsNoTracking()
            .Where(p => p.TeacherId == teacherId && !p.IsDeleted && halaqaIds.Contains(p.HalqaId))
            .Select(p => new TeacherDashboardProgressRow(p.Id, p.StudentId, p.HalqaId, p.JuzNumber, p.PageNumber, p.MemorizedUntil, p.NextTarget, p.LevelScore, p.Date))
            .ToListAsync();
}

static async Task<List<TeacherDashboardAttendanceRow>> LoadTeacherAttendanceRowsAsync(AppDbContext context, Guid teacherId, List<Guid> studentIds)
{
    return studentIds.Count == 0
        ? new List<TeacherDashboardAttendanceRow>()
        : await context.Attendances.AsNoTracking()
            .Where(a => a.TeacherId == teacherId && studentIds.Contains(a.StudentId))
            .Select(a => new TeacherDashboardAttendanceRow(a.StudentId, a.HalqeSession.HalqaId, a.Status, a.HalqeSession.date))
            .ToListAsync();
}

static Dictionary<Guid, TeacherDashboardProgressRow> GetLatestTeacherProgressByStudent(IEnumerable<TeacherDashboardProgressRow> rows)
{
    return rows.GroupBy(p => p.StudentId).ToDictionary(g => g.Key, g => g.OrderByDescending(p => p.Date).First());
}

static int TeacherAttendanceRate(IEnumerable<AttendanceStatus> statuses)
{
    var list = statuses.ToList();
    if (list.Count == 0)
        return 0;

    var attended = list.Count(status => status == AttendanceStatus.Present || status == AttendanceStatus.Late);
    return (int)Math.Round(attended * 100.0 / list.Count);
}

static int TeacherMemorizationPercent(int value)
{
    return value <= 0 ? 0 : Math.Clamp((int)Math.Round(value * 100.0 / 604), 0, 100);
}

static List<TeacherDashboardStudentAlertDto> BuildTeacherFollowUpStudents(
    List<TeacherDashboardStudentRow> students,
    Func<Guid?, string> halaqaName,
    List<TeacherDashboardProgressRow> progress,
    List<TeacherDashboardAttendanceRow> attendance)
{
    var recentStart = DateTime.UtcNow.Date.AddDays(-30);
    var followUpStart = DateTime.UtcNow.Date.AddDays(-14);
    var result = attendance
        .Where(a => a.Date >= recentStart && a.Status == AttendanceStatus.Absent)
        .GroupBy(a => a.StudentId)
        .OrderByDescending(g => g.Count())
        .Take(5)
        .Select(g =>
        {
            var student = students.FirstOrDefault(s => s.Id == g.Key);
            return new TeacherDashboardStudentAlertDto
            {
                StudentId = g.Key,
                StudentName = student?.Name ?? string.Empty,
                HalqaName = halaqaName(student?.HalqaId),
                Reason = "غياب متكرر خلال آخر 30 يوم",
                Count = g.Count()
            };
        })
        .ToList();

    if (result.Count < 5)
    {
        result.AddRange(students
            .Where(s => !progress.Any(p => p.StudentId == s.Id && p.Date >= followUpStart) && result.All(r => r.StudentId != s.Id))
            .Take(5 - result.Count)
            .Select(s => new TeacherDashboardStudentAlertDto
            {
                StudentId = s.Id,
                StudentName = s.Name,
                HalqaName = halaqaName(s.HalqaId),
                Reason = "لا يوجد تقدم مسجل خلال آخر 14 يوم",
                Count = 0
            }));
    }

    return result;
}

internal sealed record TeacherDashboardStudentRow(Guid Id, string Name, Guid? HalqaId, int Score);
internal sealed record TeacherDashboardStudentLookup(Guid Id, string Name, Guid? HalqaId, Guid? SaturdayHalqaId, Guid SaturdayHalqeId, int Score);
internal sealed record TeacherDashboardProgressRow(Guid Id, Guid StudentId, Guid HalqaId, int JuzNumber, int PageNumber, int MemorizedUntil, int NextTarget, int LevelScore, DateTime Date);
internal sealed record TeacherDashboardAttendanceRow(Guid StudentId, Guid HalqaId, AttendanceStatus Status, DateTime Date);

