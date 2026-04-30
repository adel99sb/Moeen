using System.Collections.Generic;

namespace Moeen.Shared.Responses.Library
{
    public class SearchLibraryResponse
    {
        public List<BookResponseDto> Books { get; set; }
        public int TotalCount { get; set; }
    }
}