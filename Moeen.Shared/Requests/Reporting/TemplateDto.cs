using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class TemplateDto
    {
        [StringLength(200, ErrorMessage = "Template name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;

        public string HeaderText { get; set; } = string.Empty;
        public string FooterText { get; set; } = string.Empty;
        public string LogoUrl { get; set; } = string.Empty;
        public bool IncludeCharts { get; set; }
        public string ColorScheme { get; set; } = string.Empty;
    }
}