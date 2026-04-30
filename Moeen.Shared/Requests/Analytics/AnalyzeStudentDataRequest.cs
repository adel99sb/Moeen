using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class AnalyzeStudentDataRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid Id { get; set; }
    }
}