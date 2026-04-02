using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Analytics
{
    public class AnalyzeStudentDataRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid Id { get; set; }
    }
}