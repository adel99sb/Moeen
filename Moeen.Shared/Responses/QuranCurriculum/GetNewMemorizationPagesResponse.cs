using System.Collections.Generic;

namespace Moeen.Shared.Responses.QuranCurriculum
{
    public class GetNewMemorizationPagesResponse
    {
        public List<PageDto> Pages { get; set; }
        public int NextPageNumber { get; set; } // رقم الصفحة التالية المقترحة
    }
}