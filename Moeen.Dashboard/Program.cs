using Moeen.Dashboard.Components;
using Moeen.Dashboard.Infrastructure.Http;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Infrastructure.Http.Handlers;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Dashboard.Services.Implementations;


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
var app = builder.Build();
// تسجيل خدمات المنشورات والمحتوى الجديدة
builder.Services.AddScoped<PostApiClient>();
builder.Services.AddScoped<IPostService, PostService>();
//خدمة الاختبارات
builder.Services.AddScoped<ExamCommandClient>();
builder.Services.AddScoped<IExamCommandService, ExamCommandService>();

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
