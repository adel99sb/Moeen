using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ImportExport;
using Moeen.Api.Shared.Responses.ImportExport;
using System.Threading.Tasks;

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
        /// تصدير سجل طالب
        /// </summary>
        [HttpPost("export")]
        public async Task<IActionResult> ExportStudentRecord(ExportStudentRecordRequest request)
        {
            var result = await _importExportService.ExportStudentRecordAsync(request);
            // نعيد الملف للتحميل
            return File(result.FileContent, result.ContentType, result.FileName);
        }

        /// <summary>
        /// استيراد سجل طالب
        /// </summary>
        [HttpPost("import")]
        public async Task<ActionResult<ImportStudentRecordResponse>> ImportStudentRecord(ImportStudentRecordRequest request)
        {
            var result = await _importExportService.ImportStudentRecordAsync(request);
            return Ok(result);
        }
    }
}