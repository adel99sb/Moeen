using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.StudentNotes
{
    public class UpdateStudentNoteRequest
    {
        [Required(ErrorMessage = "ãÚÑİ ÇáãáÇÍÙÉ ãØáæÈ")]
        public Guid NoteId { get; set; }

        [Required(ErrorMessage = "äÕ ÇáãáÇÍÙÉ ãØáæÈ")]
        [StringLength(2000, ErrorMessage = "äÕ ÇáãáÇÍÙÉ Øæíá ÌÏğÇ")]
        public string Text { get; set; }
    }
}