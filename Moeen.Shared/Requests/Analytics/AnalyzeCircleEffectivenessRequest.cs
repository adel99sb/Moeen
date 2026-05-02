using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class AnalyzeCircleEffectivenessRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid Id { get; set; }
    }
}