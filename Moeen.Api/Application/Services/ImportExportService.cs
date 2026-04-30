using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.ImportExport;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ImportExport;

namespace Moeen.Api.Application.Services
{
    public class ImportExportService : IImportExportService
    {
        public Task<OperationResponseDto> CancelJobAsync(CancelJobRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExportStudentRecordResponse> ExportStudentRecordAsync(ExportStudentRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ImportExportJobStatusDto> GetJobStatusAsync(GetJobStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ImportStudentRecordResponse> ImportStudentRecordAsync(ImportStudentRecordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
