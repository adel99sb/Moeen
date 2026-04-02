using System;
using System.ComponentModel.DataAnnotations;
using Moeen.Api.Core.Enums;

namespace Moeen.Api.Shared.Requests.ImportExport
{
    public class ExportStudentRecordRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Export format is required")]
        public ExportFormat Format { get; set; }
    }
}