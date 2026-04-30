using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Reporting
{
    public class CustomizeReportTemplateRequest
    {
        [Required(ErrorMessage = "Template data is required")]
        public TemplateDto Template { get; set; }
    }
}