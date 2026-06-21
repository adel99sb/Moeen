using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IParentMobileProgressService
    {
        Task<GeneralResponse> GetProgressAsync(Guid parentId, Guid? childId, DateTime? from, DateTime? to, ProgressRecordType? type);
    }
}
