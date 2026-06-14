using Moeen.Dashboard.Application.Services.Abstractions;
using Moeen.Dashboard.Application.Services.Implementations;
using Moeen.Dashboard.Components;
using Moeen.Dashboard.Infrastructure.Http;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Infrastructure.Http.Handlers;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Dashboard.Services.Implementation;
using Moeen.Dashboard.Services.Implementations;
using Moeen.Frontend.Services.Abstractions;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddTransient<AuthHandler>();

builder.Services.AddHttpClient<AuthApiClient>(c =>
{
    c.BaseAddress = new Uri(ApiRoutes.BaseUrl);
});
builder.Services.AddHttpClient<MosquApiClient>(c =>
{
    c.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IAuthService, AuthService>();
// تسجيل خدمات المنشورات والمحتوى الجديدة
//builder.Services.AddScoped<PostApiClient>();
builder.Services.AddHttpClient<PostApiClient>(client =>
{
    // هنا بنخليه ياخذ نفس الرابط الأساسي المتخزن بملف الإعدادات عندكِ
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IPostService, PostService>();
//خدمة الاختبارات
builder.Services.AddHttpClient<ExamCommandApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IExamCommandService, ExamCommandService>();
// --- Exam Halqa Services (خدمات امتحانات الحلقات) ---
builder.Services.AddHttpClient<ExamHalqaApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IExamHalqaService, ExamHalqaService>();
// EnrollmentApiClient: مسؤول عن خدمة التسجيل وادارة الاعضاء 
builder.Services.AddHttpClient<EnrollmentApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// تسجيل خدمة إدارة مراحل الاختبارات (ExamPhase) في حاوية حقن التبعية (Dependency Injection)
builder.Services.AddHttpClient<ExamPhaseApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IExamPhaseService, ExamPhaseService>();
//حقن exam query
builder.Services.AddHttpClient<ExamQueryApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IExamQueryService, ExamQueryService>();
//حقن ادارة الشكاوي والافتراحات feedback
builder.Services.AddHttpClient<FeedbackApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IFeedbackService, FeedbackService>();
//goal trackingخدمة المتابعة اليومية
builder.Services.AddHttpClient<GoalApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IGoalService, GoalService>();
//Mosqu حقن خدمة المساجد
//builder.Services.AddScoped<MosquApiClient>();
builder.Services.AddScoped<IMosquService, MosquService>();
//point حقن خدمة النقاط
builder.Services.AddHttpClient<PointsApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();
builder.Services.AddScoped<IPointsService, PointsService>();

// 1. تسجيل الـ HttpClient الخاص بالـ User وتحديد الرابط الأساسي من الـ ApiRoutes عندكِ
builder.Services.AddHttpClient<UserApiClient>(client =>
{
    client.BaseAddress = new Uri(ApiRoutes.BaseUrl);
}).AddHttpMessageHandler<AuthHandler>();

// 2. ربط الواجهة بالتنفيذ الفعلي ليتم حقنها في صفحات الـ Razor
builder.Services.AddScoped<IUserService, UserService>();

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
