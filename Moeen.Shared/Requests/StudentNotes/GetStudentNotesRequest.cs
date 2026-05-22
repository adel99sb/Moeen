using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.StudentNotes
{
    public class GetStudentNotesRequest
    {
        [Required(ErrorMessage = "ãÚÑİ ÇáØÇáÈ ãØáæÈ")]
        public Guid StudentId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}