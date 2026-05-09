using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.LessonManagement
{
    public class RecordAttendanceRequest
    {
        [Required(ErrorMessage = "Lesson ID is required")]
        public Guid LessonId { get; set; }

        [Required(ErrorMessage = "Date is required")]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Entries are required")]
        [MinLength(1)]
        public List<LessonAttendanceEntryDto> Entries { get; set; } = new();
    }

    public class LessonAttendanceEntryDto
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public AttendanceStatus Status { get; set; }

        public int? Points { get; set; }
        public string? Note { get; set; }
    }
}