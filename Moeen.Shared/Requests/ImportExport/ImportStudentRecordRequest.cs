using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ImportExport
{
    public class ImportStudentRecordRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "File data is required")]
        public byte[] Data { get; set; }
    }
}