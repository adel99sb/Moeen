using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Library
{
    public class GetBookByIdRequest
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }
    }
}