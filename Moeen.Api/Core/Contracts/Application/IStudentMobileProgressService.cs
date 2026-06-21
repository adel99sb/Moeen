using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IStudentMobileProgressService
    {
        Task<GeneralResponse> GetProgressAsync(Guid studentId, DateTime? from, DateTime? to, ProgressRecordType? type);
    }
}
