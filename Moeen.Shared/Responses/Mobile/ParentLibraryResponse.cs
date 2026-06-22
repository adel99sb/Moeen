using Moeen.Shared.Responses.Library;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class ParentLibraryResponse
    {
        public string Query { get; set; } = string.Empty;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<BookResponseDto> Books { get; set; } = new();
    }
}
