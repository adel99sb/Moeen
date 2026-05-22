using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.StudentNotes
{
    public class AddStudentNoteRequest
    {
        [Required(ErrorMessage = "ãÚÑİ ÇáØÇáÈ ãØáæÈ")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "äÕ ÇáãáÇÍÙÉ ãØáæÈ")]
        [StringLength(2000, ErrorMessage = "äÕ ÇáãáÇÍÙÉ Øæíá ÌÏğÇ")]
        public string Text { get; set; }

        public DateTime? Date { get; set; }
    }
}