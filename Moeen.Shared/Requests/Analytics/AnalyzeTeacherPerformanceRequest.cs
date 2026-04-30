using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class AnalyzeTeacherPerformanceRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid Id { get; set; }
    }
}