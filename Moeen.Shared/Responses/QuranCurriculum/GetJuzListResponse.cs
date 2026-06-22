using System.Collections.Generic;

namespace Moeen.Shared.Responses.QuranCurriculum
{
    public class GetJuzListResponse
    {
        public List<JuzDto> JuzList { get; set; } = new List<JuzDto>();
        public int TotalCount { get; set; }
    }
}