using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.QuranCurriculum
{
    public class GetNewMemorizationPagesRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Range(1, 10, ErrorMessage = "Number of pages must be between 1 and 10")]
        public int NumberOfPages { get; set; } = 1;
    }
}