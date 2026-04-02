using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.QuranCurriculum
{
    public class GetPagesByJuzResponse
    {
        public int JuzNumber { get; set; }
        public List<PageDto> Pages { get; set; }
    }
}