using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Analytics
{
    public class AnalyzeCircleEffectivenessRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid Id { get; set; }
    }
}