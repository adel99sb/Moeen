using Moeen.Dashboard.Components;



var builder = WebApplication.CreateBuilder(args);

var apiBaseUrl = builder.Configuration["ApiBaseUrl"] ?? "https://localhost:7023/";


builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient("MoeenApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("MoeenApi"));
builder.Services.AddScoped<Moeen.Dashboard.Services.Abstractions.IUserApiService, Moeen.Dashboard.Services.Implementations.UserApiService>();
builder.Services.AddScoped<Moeen.Dashboard.Services.Abstractions.IMosqueApiService, Moeen.Dashboard.Services.Implementations.MosqueApiService>();
builder.Services.AddScoped<Moeen.Dashboard.Services.Abstractions.IComplaintApiService, Moeen.Dashboard.Services.Implementations.ComplaintApiService>();
builder.Services.AddScoped<Moeen.Dashboard.Services.Abstractions.IAuthApiService, Moeen.Dashboard.Services.Implementations.AuthApiService>();
builder.Services.AddScoped<Moeen.Dashboard.Services.Abstractions.IAuthSessionService, Moeen.Dashboard.Services.Implementations.AuthSessionService>();



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
