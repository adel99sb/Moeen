using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Memorization;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Memorization;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/memorization-management")]
    [ApiController]
    public class MemorizationManagementController : ControllerBase
    {
        private readonly IMemorizationService _memorizationService;

        public MemorizationManagementController(IMemorizationService memorizationService)
        {
            _memorizationService = memorizationService;
        }

        [HttpPost("record-page")]
        public async Task<ActionResult<RecordPageMemorizationResponse>> RecordNewPage([FromBody] RecordPageMemorizationRequest request)
            => Ok(await _memorizationService.RecordNewPageMemorizationAsync(request));

        [HttpPost("record-batch")]
        public async Task<ActionResult<BatchResult>> RecordPagesBatch([FromBody] RecordPagesBatchRequest request)
            => Ok(await _memorizationService.RecordNewPagesBatchAsync(request));

        [HttpGet("students/{studentId:guid}/last-page")]
        public async Task<ActionResult<GetLastMemorizedPageResponse>> GetLastMemorizedPage([FromRoute] Guid studentId)
            => Ok(await _memorizationService.GetLastMemorizedPageAsync(new GetLastMemorizedPageRequest { StudentId = studentId }));

        [HttpGet("students/{studentId:guid}/records/{pageNumber:int}")]
        public async Task<ActionResult<MemorizationRecordDto>> GetMemorizationRecord([FromRoute] Guid studentId, [FromRoute] int pageNumber)
            => Ok(await _memorizationService.GetMemorizationRecordAsync(new GetMemorizationRecordRequest
            {
                StudentId = studentId,
                PageNumber = pageNumber
            }));

        [HttpGet("students/{studentId:guid}/history")]
        public async Task<ActionResult<PagedList<MemorizationRecordDto>>> GetStudentMemorizationHistory(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
            => Ok(await _memorizationService.GetStudentMemorizationHistoryAsync(new GetStudentMemorizationHistoryRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate,
                PageNumber = pageNumber,
                PageSize = pageSize
            }));

        [HttpGet("students/{studentId:guid}/statistics")]
        public async Task<ActionResult<MemorizationStatisticsDto>> GetMemorizationStatistics(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
            => Ok(await _memorizationService.GetMemorizationStatisticsAsync(new GetMemorizationStatisticsRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpGet("circles/{circleId:guid}/progress")]
        public async Task<ActionResult<List<StudentMemorizationSummaryDto>>> GetCircleMemorizationProgress([FromRoute] Guid circleId)
            => Ok(await _memorizationService.GetCircleMemorizationProgressAsync(new GetCircleProgressRequest { CircleId = circleId }));

        [HttpGet("students/{studentId:guid}/progress-report")]
        public async Task<ActionResult<MemorizationProgressReportDto>> GetProgressReport(
            [FromRoute] Guid studentId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
            => Ok(await _memorizationService.GetMemorizationProgressReportAsync(new GetProgressReportRequest
            {
                StudentId = studentId,
                FromDate = fromDate,
                ToDate = toDate
            }));

        [HttpPut("records/{recordId:guid}/grade")]
        public async Task<ActionResult<MemorizationRecordDto>> UpdateMemorizationGrade(
            [FromRoute] Guid recordId,
            [FromBody] UpdateMemorizationGradeRequest request)
        {
            request.RecordId = recordId;
            return Ok(await _memorizationService.UpdateMemorizationGradeAsync(request));
        }

        [HttpPut("records/{recordId:guid}/reset")]
        public async Task<ActionResult<OperationResponseDto>> ResetMemorizationRecord(
            [FromRoute] Guid recordId,
            [FromBody] ResetMemorizationRecordRequest request)
        {
            request.RecordId = recordId;
            return Ok(await _memorizationService.ResetMemorizationRecordAsync(request));
        }

        [HttpDelete("records/{recordId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteMemorizationRecord([FromRoute] Guid recordId)
            => Ok(await _memorizationService.DeleteMemorizationRecordAsync(new DeleteMemorizationRecordRequest { RecordId = recordId }));

        [HttpPost("certificates")]
        public async Task<ActionResult<CertificateDto>> GenerateMemorizationCertificate([FromBody] GenerateCertificateRequest request)
            => Ok(await _memorizationService.GenerateMemorizationCertificateAsync(request));
    }
}