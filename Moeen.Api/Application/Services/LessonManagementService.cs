using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.LessonManagement;

namespace Moeen.Api.Application.Services
{
    public class LessonManagementService : ILessonManagementService
    {
        public Task<AddLessonMaterialsResponse> AddLessonMaterialsAsync(AddLessonMaterialsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> AssignLessonToCirclesAsync(AssignLessonToCirclesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> AssignTeacherToLessonAsync(AssignTeacherToLessonRequest request)
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

        public Task<LessonDto> DuplicateLessonAsync(DuplicateLessonRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonDto> GetLessonByIdAsync(GetLessonByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<LessonMaterialDto>> GetLessonMaterialsAsync(GetLessonMaterialsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<LessonDto>> GetLessonsByCircleAsync(GetLessonsByCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonScheduleDto> GetLessonScheduleAsync(GetLessonScheduleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ManageLessonTimeResponse> ManageLessonTimeAsync(ManageLessonTimeRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonAttendanceDto> RecordLessonAttendanceAsync(RecordAttendanceRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReorderLessonsResponse> ReorderLessonsAsync(ReorderLessonsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> SoftDeleteLessonAsync(SoftDeleteLessonRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> UnassignLessonFromCircleAsync(UnassignLessonFromCircleRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonDto> UpdateLessonAsync(UpdateLessonRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<LessonScheduleDto> UpdateLessonScheduleAsync(UpdateLessonScheduleRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
