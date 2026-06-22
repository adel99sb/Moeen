using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Library
{
    public class AddBookRequest
    {
        [Required(ErrorMessage = "Book data is required")]
        public BookDto BookData { get; set; } = null!;

        public byte[] BookFile { get; set; }
    }
}