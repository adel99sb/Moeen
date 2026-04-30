using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.ImportExport;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ImportExport;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImportExportController : ControllerBase
    {
        private readonly IImportExportService _importExportService;

        public ImportExportController(IImportExportService importExportService)
        {
            _importExportService = importExportService;
        }

        /// <summary>
        /// POST (قديم/متوافق): تصدير سجل طالب.
        /// </summary>
        [HttpPost("export")]
        public async Task<IActionResult> ExportStudentRecord([FromBody] ExportStudentRecordRequest request)
        {
            var result = await _importExportService.ExportStudentRecordAsync(request);
            return File(result.FileContent, result.ContentType, result.FileName);
        }

        /// <summary>
        /// GET (جديد): تصدير سجل طالب عبر Route/Query.
        /// </summary>
        [HttpGet("students/{studentId:guid}/export")]
        public async Task<IActionResult> ExportStudentRecordGet([FromRoute] Guid studentId, [FromQuery] ExportFormat format)
        {
            var request = new ExportStudentRecordRequest
            {
                StudentId = studentId,
                Format = format
            };

            var result = await _importExportService.ExportStudentRecordAsync(request);
            return File(result.FileContent, result.ContentType, result.FileName);
        }

        /// <summary>
        /// أمر: استيراد سجل طالب.
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ImportStudentRecordResponse>> ImportStudentRecord([FromBody] ImportStudentRecordRequest request)
            => Ok(await _importExportService.ImportStudentRecordAsync(request));

        /// <summary>
        /// DELETE: إلغاء عملية Import/Export قيد التنفيذ.
        /// </summary>
        [HttpDelete("jobs/{jobId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> CancelJob([FromRoute] Guid jobId, [FromQuery] string? reason)
            => Ok(await _importExportService.CancelJobAsync(new CancelJobRequest
            {
                JobId = jobId,
                Reason = reason
            }));

        /// <summary>
        /// GET: جلب حالة عملية Import/Export.
        /// </summary>
        [HttpGet("jobs/{jobId:guid}/status")]
        public async Task<ActionResult<ImportExportJobStatusDto>> GetJobStatus([FromRoute] Guid jobId)
            => Ok(await _importExportService.GetJobStatusAsync(new GetJobStatusRequest { JobId = jobId }));
    }
}