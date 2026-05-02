using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Memorization;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Memorization;

namespace Moeen.Api.Application.Services
{
    public class MemorizationService : IMemorizationService
    {
        public Task<OperationResponseDto> DeleteMemorizationRecordAsync(DeleteMemorizationRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CertificateDto> GenerateMemorizationCertificateAsync(GenerateCertificateRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudentMemorizationSummaryDto>> GetCircleMemorizationProgressAsync(GetCircleProgressRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetLastMemorizedPageResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemorizationProgressReportDto> GetMemorizationProgressReportAsync(GetProgressReportRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemorizationRecordDto> GetMemorizationRecordAsync(GetMemorizationRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemorizationStatisticsDto> GetMemorizationStatisticsAsync(GetMemorizationStatisticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<MemorizationRecordDto>> GetStudentMemorizationHistoryAsync(GetStudentMemorizationHistoryRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<RecordPageMemorizationResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<BatchResult> RecordNewPagesBatchAsync(RecordPagesBatchRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> ResetMemorizationRecordAsync(ResetMemorizationRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemorizationRecordDto> UpdateMemorizationGradeAsync(UpdateMemorizationGradeRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
