namespace Moeen.Api.infrastructure.Configurations
{
    public sealed class AppSettings
    {
        private static readonly Lazy<AppSettings> _instance = new(() => new AppSettings());
        public static AppSettings Instance => _instance.Value;

        public JwtSettings JwtSettings { get; }
        public SmtpSettings SmtpSettings { get; }
        public string ConnectionString { get; }
        public bool ApplyMigrationsOnStartup { get; }
        public bool UseHttpsRedirection { get; }

        private AppSettings()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            JwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
            SmtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>() ?? new SmtpSettings();
            ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            ApplyMigrationsOnStartup = configuration.GetValue<bool>("DatabaseSettings:ApplyMigrationsOnStartup");
            UseHttpsRedirection = configuration.GetValue<bool>("Hosting:UseHttpsRedirection");
        }
    }
}
