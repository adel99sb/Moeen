using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IStudentMobilePostService
    {
        Task<GeneralResponse> GetPostsAsync(Guid studentId);
        Task<GeneralResponse> ToggleLikeAsync(Guid studentId, Guid postId);
    }
}
