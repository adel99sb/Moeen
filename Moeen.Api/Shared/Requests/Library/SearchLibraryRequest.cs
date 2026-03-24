using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Library
{
    public class SearchLibraryRequest
    {
        [Required(ErrorMessage = "Query is required")]
        [StringLength(200, MinimumLength = 1, ErrorMessage = "Query must be between 1 and 200 characters")]
        public string Query { get; set; }
    }
}