using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IParentMobilePostService
    {
        Task<GeneralResponse> GetPostsAsync(Guid parentId);
        Task<GeneralResponse> ToggleLikeAsync(Guid parentId, Guid postId);
    }
}
