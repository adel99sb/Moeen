
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Exam_Halqa;

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
            public async Task<IActionResult> CreateAsync([FromBody] CreateExamTeacherRequest request)
            {
                var response = await _examHalqaService.CreateAsync(request);
                return Ok(response);
            }

            /// <summary>
            /// جلب بيانات أستاذ اختبار حسب الـ ID
            /// </summary>
            [HttpGet("get-by-id/{id}")]
            public async Task<IActionResult> GetByIdAsync(int id)
            {
                var request = new GetExamTeacherByIdRequest { Id = id };
                var response = await _examHalqaService.GetByIdAsync(request);
                return Ok(response);
            }

            /// <summary>
            /// ربط حلقة بأستاذ الاختبار
            /// </summary>
            [HttpPost("assign-halqa")]
            public async Task<IActionResult> AssignHalqaAsync([FromBody] AssignHalqaToExamTeacherRequest request)
            {
                var response = await _examHalqaService.AssignHalqaAsync(request);
                return Ok(response);
            }
        }
    }