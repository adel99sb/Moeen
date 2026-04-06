using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.LessonManagement;
using Moeen.Api.Shared.Responses.LessonManagement;

namespace Moeen.Api.Application.Services
{
    public class LessonManagementService : ILessonManagementService
    {
        public Task<AddLessonMaterialsResponse> AddLessonMaterialsAsync(AddLessonMaterialsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<CopyLessonsResponse> CopyLessonsAsync(CopyLessonsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonDto> CreateLessonAsync(CreateLessonRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteLessonResponse> DeleteLessonAsync(DeleteLessonRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ManageLessonTimeResponse> ManageLessonTimeAsync(ManageLessonTimeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReorderLessonsResponse> ReorderLessonsAsync(ReorderLessonsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonDto> UpdateLessonAsync(UpdateLessonRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
