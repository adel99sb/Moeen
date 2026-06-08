using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.DailyAssignments
{
    public class AddDailyAssignmentRequest
    {
        [Required(ErrorMessage = "Student IDs are required")]
        [MinLength(1)]
        public List<Guid> StudentIds { get; set; } = new();

        [Required(ErrorMessage = "Assignment type is required")]
        public ProgressRecordType AssignmentType { get; set; }

        public DateTime? Date { get; set; }
        public int? JuzNumber { get; set; }
        public int? FromPage { get; set; }
        public int? ToPage { get; set; }
        public string? Notes { get; set; }
    }
}