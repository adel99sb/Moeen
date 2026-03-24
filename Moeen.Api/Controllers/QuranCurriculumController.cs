using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.QuranCurriculum;
using Moeen.Api.Shared.Responses.QuranCurriculum;
using System.Threading.Tasks;

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

        /// <summary>
        /// تهيئة المصحف (جزء وصفحات)
        /// </summary>
        [HttpPost("initialize")]
        public async Task<ActionResult<InitializeQuranResponse>> InitializeQuran(InitializeQuranRequest request)
        {
            var result = await _quranCurriculumService.InitializeQuranAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على قائمة الأجزاء
        /// </summary>
        [HttpPost("juz-list")]
        public async Task<ActionResult<GetJuzListResponse>> GetJuzList(GetJuzListRequest request)
        {
            var result = await _quranCurriculumService.GetJuzListAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على صفحات جزء معين
        /// </summary>
        [HttpPost("pages-by-juz")]
        public async Task<ActionResult<GetPagesByJuzResponse>> GetPagesByJuz(GetPagesByJuzRequest request)
        {
            var result = await _quranCurriculumService.GetPagesByJuzAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على صفحات جديدة للحفظ لطالب معين
        /// </summary>
        [HttpPost("new-memorization-pages")]
        public async Task<ActionResult<GetNewMemorizationPagesResponse>> GetNewMemorizationPages(GetNewMemorizationPagesRequest request)
        {
            var result = await _quranCurriculumService.GetNewMemorizationPagesAsync(request);
            return Ok(result);
        }
    }
}