using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class TemplateDto
    {
        [StringLength(200, ErrorMessage = "Template name cannot exceed 200 characters")]
        public string Name { get; set; }

        public string HeaderText { get; set; }
        public string FooterText { get; set; }
        public string LogoUrl { get; set; }
        public bool IncludeCharts { get; set; }
        public string ColorScheme { get; set; }
    }
}