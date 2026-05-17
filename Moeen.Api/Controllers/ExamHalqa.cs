using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamHalqaController : ControllerBase
    {
        private readonly IExamHalqaService _examHalqaService;

        public ExamHalqaController(IExamHalqaService examHalqaService)
        {
            _examHalqaService = examHalqaService;
        }

        /// <summary>
        /// إنشاء أستاذ اختبارات جديد
        /// </summary>
        [HttpPost("create")]
        public async Task<ActionResult<GeneralResponse>> CreateAsync([FromBody] CreateExamTeacherRequest request)
        {
            var response = await _examHalqaService.CreateAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// جلب بيانات أستاذ اختبار حسب الـ ID
        /// </summary>
        [HttpGet("get-by-id/{id:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetByIdAsync([FromRoute] Guid id)
        {
            var request = new GetExamTeacherByIdRequest { Id = id };
            var response = await _examHalqaService.GetByIdAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        /// <summary>
        /// ربط حلقات بأستاذ الاختبار
        /// </summary>
        [HttpPost("assign-halqa")]
        public async Task<ActionResult<GeneralResponse>> AssignHalqaAsync([FromBody] AssignHalqaToExamTeacherRequest request)
        {
            var response = await _examHalqaService.AssignHalqaAsync(request);
            return StatusCode(response.StatusCode, response);
        }
    }
}