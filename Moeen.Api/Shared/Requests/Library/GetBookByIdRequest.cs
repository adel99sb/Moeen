using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Library
{
    public class GetBookByIdRequest
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }
    }
}