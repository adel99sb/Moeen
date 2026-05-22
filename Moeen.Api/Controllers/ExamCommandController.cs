using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Responses;
using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ExamCommand;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamCommandController : ControllerBase
    {
        private readonly IExamCommandService _examCommandService;

        public ExamCommandController(IExamCommandService examCommandService)
        {
            _examCommandService = examCommandService;
        }

        /// <summary>
        /// أمر: تسجيل اختبار جديد.
        /// </summary>
        [HttpPost("register")]
        public async Task<ActionResult<GeneralResponse>> RegisterExam([FromBody] RegisterExamRequest request)
        {
            try
            {
                var result = await _examCommandService.RegisterExamAsync(request);
                return Ok(GeneralResponse.Ok("تم تسجيل الاختبار بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء تسجيل الاختبار"));
            }
        }

        /// <summary>
        /// أمر: تحديث نتيجة اختبار.
        /// </summary>
        [HttpPut("update-result")]
        public async Task<ActionResult<GeneralResponse>> UpdateExamResult([FromBody] UpdateExamResultRequest request)
        {
            try
            {
                var result = await _examCommandService.UpdateExamResultAsync(request);
                return Ok(GeneralResponse.Ok("تم تحديث نتيجة الاختبار بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء تحديث النتيجة"));
            }
        }

        /// <summary>
        /// أمر: تحديث البيانات الوصفية للاختبار (التاريخ، النوع، الملاحظات).
        /// </summary>
        [HttpPut("update-info")]
        public async Task<ActionResult<GeneralResponse>> UpdateExamInfo([FromBody] UpdateExamInfoRequest request)
        {
            try
            {
                var result = await _examCommandService.UpdateExamInfoAsync(request);
                return Ok(GeneralResponse.Ok("تم تحديث بيانات الاختبار بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء تحديث البيانات"));
            }
        }

        /// <summary>
        /// أمر: إضافة ملاحظات تقييمية وتوصيات على الاختبار.
        /// </summary>
        [HttpPut("add-feedback")]
        public async Task<ActionResult<GeneralResponse>> AddExamFeedback([FromBody] AddExamFeedbackRequest request)
        {
            try
            {
                var result = await _examCommandService.AddExamFeedbackAsync(request);
                return Ok(GeneralResponse.Ok("تم إضافة ملاحظات الامتحان بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء إضافة الملاحظات"));
            }
        }

        /// <summary>
        /// أمر: حذف نتيجة اختبار.
        /// </summary>
        [HttpDelete("delete")]
        public async Task<ActionResult<GeneralResponse>> DeleteExamResult([FromBody] DeleteExamResultRequest request)
        {
            try
            {
                var result = await _examCommandService.DeleteExamResultAsync(request);
                return Ok(GeneralResponse.Ok(result.Message ?? "تم حذف نتيجة الاختبار بنجاح.", result));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(GeneralResponse.BadRequest(ex.Message));
            }
            catch (Exception)
            {
                return StatusCode(500, GeneralResponse.InternalError("حدث خطأ داخلي أثناء حذف النتيجة"));
            }
        }

        /// <summary>
        /// تسجيل اختبار مختبر جديد.
        /// </summary>
        [HttpPost("create-lab-exam")]
        public async Task<ActionResult<GeneralResponse>> CreateLabExam([FromBody] CreateLabExamRequest request)
        {
            var result = await _examCommandService.CreateLabExamAsync(request);
            return StatusCode(result.StatusCode, result);
        }
    }
}