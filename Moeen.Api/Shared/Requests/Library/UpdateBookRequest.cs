using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Library
{
    public class UpdateBookRequest
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Book data is required")]
        public BookDto BookData { get; set; }
    }
}