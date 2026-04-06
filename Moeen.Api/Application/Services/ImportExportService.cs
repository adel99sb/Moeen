using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.ImportExport;
using Moeen.Api.Shared.Responses.ImportExport;

namespace Moeen.Api.Application.Services
{
    public class ImportExportService : IImportExportService
    {
        public Task<ExportStudentRecordResponse> ExportStudentRecordAsync(ExportStudentRecordRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ImportStudentRecordResponse> ImportStudentRecordAsync(ImportStudentRecordRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
