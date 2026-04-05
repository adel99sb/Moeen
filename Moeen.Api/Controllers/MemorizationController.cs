using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Memorization;
using Moeen.Api.Shared.Responses.Memorization;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemorizationController : ControllerBase
    {
        private readonly IMemorizationService _memorizationService;

        public MemorizationController(IMemorizationService memorizationService)
        {
            _memorizationService = memorizationService;
        }

        /// <summary>
        /// تسجيل حفظ صفحة جديدة
        /// </summary>
        [HttpPost("record-page")]
        public async Task<ActionResult<RecordPageMemorizationResponse>> RecordNewPage(RecordPageMemorizationRequest request)
        {
            var result = await _memorizationService.RecordNewPageMemorizationAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل حفظ مجموعة صفحات دفعة واحدة
        /// </summary>
        [HttpPost("record-batch")]
        public async Task<ActionResult<BatchResult>> RecordPagesBatch(RecordPagesBatchRequest request)
        {
            var result = await _memorizationService.RecordNewPagesBatchAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على آخر صفحة قام الطالب بحفظها
        /// </summary>
        [HttpPost("last-page")]
        public async Task<ActionResult<GetLastMemorizedPageResponse>> GetLastMemorizedPage(GetLastMemorizedPageRequest request)
        {
            var result = await _memorizationService.GetLastMemorizedPageAsync(request);
            return Ok(result);
        }
    }
}
