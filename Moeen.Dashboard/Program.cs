using Moeen.Dashboard.Application.Services.Abstractions;
using Moeen.Dashboard.Application.Services.Implementations;
using Moeen.Dashboard.Components;
using Moeen.Dashboard.Infrastructure.Http;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Dashboard.Services.Implementations;
using Moeen.Frontend.Services.Abstractions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
//builder.Services.AddScoped<ITokenService, TokenService>();

//builder.Services.AddTransient<AuthHandler>();

builder.Services.AddHttpClient<AuthApiClient>(c =>
{
    c.BaseAddress = new Uri(ApiRoutes.BaseUrl);
})
/*.AddHttpMessageHandler<AuthHandler>()*/;
builder.Services.AddScoped<IAuthService, AuthService>();
// تسجيل خدمات المنشورات والمحتوى الجديدة
builder.Services.AddScoped<PostApiClient>();
builder.Services.AddScoped<IPostService, PostService>();
//خدمة الاختبارات
builder.Services.AddScoped<ExamCommandApiClient>();
builder.Services.AddScoped<IExamCommandService, ExamCommandService>();
// --- Exam Halqa Services (خدمات امتحانات الحلقات) ---
builder.Services.AddScoped<ExamHalqaApiClient>();
builder.Services.AddScoped<IExamHalqaService, ExamHalqaService>();
// EnrollmentApiClient: مسؤول عن خدمة التسجيل وادارة الاعضاء 
builder.Services.AddScoped<EnrollmentApiClient>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// تسجيل خدمة إدارة مراحل الاختبارات (ExamPhase) في حاوية حقن التبعية (Dependency Injection)
builder.Services.AddScoped<ExamPhaseApiClient>();
builder.Services.AddScoped<IExamPhaseService, ExamPhaseService>();
//حقن exam query
builder.Services.AddScoped<ExamQueryApiClient>();
builder.Services.AddScoped<IExamQueryService, ExamQueryService>();
//حقن ادارة الشكاوي والافتراحات feedback
builder.Services.AddScoped<FeedbackApiClient>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
//goal trackingخدمة المتابعة اليومية
builder.Services.AddScoped<GoalApiClient>();
builder.Services.AddScoped<IGoalService, GoalService>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
