using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.QuranCurriculum
{
    public class GetJuzListResponse
    {
        public List<JuzDto> JuzList { get; set; }
        public int TotalCount { get; set; }
    }
}