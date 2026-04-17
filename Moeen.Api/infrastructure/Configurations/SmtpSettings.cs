namespace Moeen.Api.infrastructure.Configurations
{
    public class SmtpSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool EnableSsl { get; set; }
        public string SenderEmail { get; set; }
        public string SenderPassword { get; set; }
    }
}
