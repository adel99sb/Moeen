using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleQuery;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleQuery;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CircleQueryController : ControllerBase
    {
        // خدمة الاستعلام الخاصة بالحلقات (طبقة التطبيق)
        private readonly ICircleQueryService _circleQueryService;

        public CircleQueryController(ICircleQueryService circleQueryService)
        {
            _circleQueryService = circleQueryService;
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب حلقة عبر DTO في Body.
        /// يستخدم للحفاظ على التوافق الخلفي مع العملاء الحاليين.
        /// </summary>
        [HttpPost("get-by-id")]
        public async Task<ActionResult<CircleDto>> GetCircleById(GetCircleByIdRequest request)
        {
            var result = await _circleQueryService.GetCircleByIdAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET (جديد): جلب حلقة مباشرة عبر Route Parameter.
        /// أفضل RESTful للقراءة البسيطة (ById).
        /// </summary>
        [HttpGet("{circleId:guid}")]
        public async Task<ActionResult<CircleDto>> GetCircleByIdGet([FromRoute] Guid circleId)
        {
            // إعادة استخدام نفس منطق الخدمة الحالي بدون تعديل العقود
            var request = new GetCircleByIdRequest { CircleId = circleId };
            var result = await _circleQueryService.GetCircleByIdAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب طلاب الحلقة مع دعم Filter معقد في Body.
        /// يبقى مفيدًا عند وجود معايير بحث متعددة.
        /// </summary>
        [HttpPost("get-students")]
        public async Task<ActionResult<CircleStudentsResponse>> GetCircleStudents(GetCircleStudentsRequest request)
        {
            var result = await _circleQueryService.GetCircleStudentsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET (جديد): جلب طلاب الحلقة بدون فلاتر معقدة.
        /// مناسب لشاشات العرض السريع/الافتراضي.
        /// </summary>
        [HttpGet("{circleId:guid}/students")]
        public async Task<ActionResult<CircleStudentsResponse>> GetCircleStudentsGet([FromRoute] Guid circleId)
        {
            var request = new GetCircleStudentsRequest { CircleId = circleId };
            var result = await _circleQueryService.GetCircleStudentsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب عدد طلاب الحلقة عبر Body.
        /// </summary>
        [HttpPost("get-students-count")]
        public async Task<ActionResult<CircleStudentsCountResponse>> GetCircleStudentsCount(GetCircleStudentsCountRequest request)
        {
            var result = await _circleQueryService.GetCircleStudentsCountAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET (جديد): جلب عدد طلاب الحلقة عبر Route.
        /// endpoint خفيف ومباشر للاستعلامات السريعة.
        /// </summary>
        [HttpGet("{circleId:guid}/students/count")]
        public async Task<ActionResult<CircleStudentsCountResponse>> GetCircleStudentsCountGet([FromRoute] Guid circleId)
        {
            var request = new GetCircleStudentsCountRequest { CircleId = circleId };
            var result = await _circleQueryService.GetCircleStudentsCountAsync(request);
            return Ok(result);
        }
    }
}