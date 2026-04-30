using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Library
{
    public class UpdateBookCategoriesRequest
    {
        [Required(ErrorMessage = "Book ID is required")]
        public Guid BookId { get; set; }

        [Required(ErrorMessage = "At least one category is required")]
        [MinLength(1, ErrorMessage = "At least one category is required")]
        public List<string> Categories { get; set; } = new();
    }
}