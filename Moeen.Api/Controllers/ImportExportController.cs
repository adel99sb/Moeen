using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ImportExport;
using Moeen.Api.Shared.Responses.ImportExport;

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
        public async Task<IActionResult> ExportStudentRecord(ExportStudentRecordRequest request)
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
        public async Task<ActionResult<ImportStudentRecordResponse>> ImportStudentRecord(ImportStudentRecordRequest request)
            => Ok(await _importExportService.ImportStudentRecordAsync(request));
    }
}