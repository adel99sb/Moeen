using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Memorization;
using Moeen.Api.Shared.Responses.Memorization;
using System;

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
            => Ok(await _memorizationService.RecordNewPageMemorizationAsync(request));

        /// <summary>
        /// تسجيل حفظ مجموعة صفحات دفعة واحدة
        /// </summary>
        [HttpPost("record-batch")]
        public async Task<ActionResult<BatchResult>> RecordPagesBatch(RecordPagesBatchRequest request)
            => Ok(await _memorizationService.RecordNewPagesBatchAsync(request));

        /// <summary>
        /// الحصول على آخر صفحة قام الطالب بحفظها
        /// </summary>
        [HttpPost("last-page")]
        public async Task<ActionResult<GetLastMemorizedPageResponse>> GetLastMemorizedPage(GetLastMemorizedPageRequest request)
            => Ok(await _memorizationService.GetLastMemorizedPageAsync(request));

        /// <summary>
        /// GET (جديد): آخر صفحة محفوظة للطالب.
        /// </summary>
        [HttpGet("students/{studentId:guid}/last-page")]
        public async Task<ActionResult<GetLastMemorizedPageResponse>> GetLastMemorizedPageGet([FromRoute] Guid studentId)
            => Ok(await _memorizationService.GetLastMemorizedPageAsync(new GetLastMemorizedPageRequest { StudentId = studentId }));
    }
}
