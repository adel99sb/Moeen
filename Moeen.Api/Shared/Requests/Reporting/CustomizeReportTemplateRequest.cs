using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Reporting
{
    public class CustomizeReportTemplateRequest
    {
        [Required(ErrorMessage = "Template data is required")]
        public TemplateDto Template { get; set; }
    }
}