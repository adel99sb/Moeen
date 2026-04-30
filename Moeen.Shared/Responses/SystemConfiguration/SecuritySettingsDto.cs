namespace Moeen.Shared.Responses.SystemConfiguration
{
    public class SecuritySettingsDto
    {
        public bool TwoFactorEnabled { get; set; }
        public int PasswordMinLength { get; set; }
        public bool RequireUppercase { get; set; }
        public bool RequireDigit { get; set; }
        public int SessionTimeoutMinutes { get; set; }
        public int MaxLoginAttempts { get; set; }
    }
}