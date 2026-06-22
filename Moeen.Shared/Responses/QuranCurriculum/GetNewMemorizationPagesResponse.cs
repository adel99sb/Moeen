using System.Collections.Generic;

namespace Moeen.Shared.Responses.QuranCurriculum
{
    public class GetNewMemorizationPagesResponse
    {
        public List<PageDto> Pages { get; set; } = new List<PageDto>();
        public int NextPageNumber { get; set; } // رقم الصفحة التالية المقترحة
    }
}