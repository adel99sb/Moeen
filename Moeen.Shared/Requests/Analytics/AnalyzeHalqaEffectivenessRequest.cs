using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class AnalyzeHalqaEffectivenessRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid Id { get; set; }
    }
}