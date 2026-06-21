using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IStudentMobileProfileService
    {
        Task<GeneralResponse> GetProfileAsync(Guid studentId);
        Task<GeneralResponse> SubmitNoteAsync(Guid studentId, SubmitStudentProfileNoteRequest request);
    }
}
