using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.QuranCurriculum;
using Moeen.Shared.Responses.QuranCurriculum;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuranCurriculumController : ControllerBase
    {
        private readonly IQuranCurriculumService _quranCurriculumService;

        public QuranCurriculumController(IQuranCurriculumService quranCurriculumService)
        {
            _quranCurriculumService = quranCurriculumService;
        }

        [HttpPost("initialize")]
        public async Task<ActionResult<InitializeQuranResponse>> InitializeQuran(InitializeQuranRequest request)
            => Ok(await _quranCurriculumService.InitializeQuranAsync(request));

        [HttpPost("juz-list")]
        public async Task<ActionResult<GetJuzListResponse>> GetJuzList(GetJuzListRequest request)
            => Ok(await _quranCurriculumService.GetJuzListAsync(request));

        /// <summary>
        /// GET (جديد): قائمة الأجزاء.
        /// </summary>
        [HttpGet("juz-list")]
        public async Task<ActionResult<GetJuzListResponse>> GetJuzListGet()
            => Ok(await _quranCurriculumService.GetJuzListAsync(new GetJuzListRequest()));

        [HttpPost("pages-by-juz")]
        public async Task<ActionResult<GetPagesByJuzResponse>> GetPagesByJuz(GetPagesByJuzRequest request)
            => Ok(await _quranCurriculumService.GetPagesByJuzAsync(request));

        /// <summary>
        /// GET (جديد): صفحات جزء محدد.
        /// </summary>
        [HttpGet("juz/{juzNumber:int}/pages")]
        public async Task<ActionResult<GetPagesByJuzResponse>> GetPagesByJuzGet([FromRoute] int juzNumber)
            => Ok(await _quranCurriculumService.GetPagesByJuzAsync(new GetPagesByJuzRequest { JuzNumber = juzNumber }));

        [HttpPost("new-memorization-pages")]
        public async Task<ActionResult<GetNewMemorizationPagesResponse>> GetNewMemorizationPages(GetNewMemorizationPagesRequest request)
            => Ok(await _quranCurriculumService.GetNewMemorizationPagesAsync(request));
    }
}