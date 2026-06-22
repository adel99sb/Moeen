namespace Moeen.Api.infrastructure.Configurations
{
    public class AppSettings
    {
        private static readonly Lazy<AppSettings> _instance =
    new Lazy<AppSettings>(() => new AppSettings());

        public static AppSettings Instance => _instance.Value;

        public JwtSettings JwtSettings { get; private set; } = null!;
        public SmtpSettings SmtpSettings { get; private set; } = null!;
        public string ConnectionString { get; private set; }

        private AppSettings()
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            var configBuilder = new ConfigurationBuilder()
                 .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                 .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                 .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true);

            IConfiguration configuration = configBuilder.Build();
            if (environment == "Production")
            {
                //JwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
                //JwtSettings.SecretKey = Environment.GetEnvironmentVariable("SecretKey") ?? JwtSettings.SecretKey;
                ////
                //SmtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>() ?? new SmtpSettings();
                //SmtpSettings.SenderPassword = Environment.GetEnvironmentVariable("SenderPassword") ?? SmtpSettings.SenderPassword;                              
                ////
                //ConnectionString = Environment.GetEnvironmentVariable("DefaultConnection") ?? configuration.GetConnectionString("DefaultConnection");
                JwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
                SmtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>() ?? new SmtpSettings();
                ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            }
            else
            {
                JwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
                SmtpSettings = configuration.GetSection("SmtpSettings").Get<SmtpSettings>() ?? new SmtpSettings();
                ConnectionString = configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
            }
        }
    }
}
