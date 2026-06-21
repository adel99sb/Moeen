using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IParentMobileProfileService
    {
        Task<GeneralResponse> GetProfileAsync(Guid parentId);
        Task<GeneralResponse> SubmitNoteAsync(Guid parentId, SubmitParentProfileNoteRequest request);
    }
}
