using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.ImportExport
{
    public class ExportStudentRecordRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Export format is required")]
        public ExportFormat Format { get; set; }
    }
}