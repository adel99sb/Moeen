using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Enrollment;

namespace Moeen.Api.Application.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        public Task<TeacherDto> AddTeacherAsync(AddTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CancelMembershipAsync(CancelMembershipRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeleteParentAsync(DeleteParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeleteStudentAsync(DeleteStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeleteTeacherAsync(DeleteTeacherRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> ExportMembersListAsync(ExportMembersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<ParentDto>> GetAllParentsAsync(GetAllParentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<StudentDto>> GetAllStudentsAsync(GetAllStudentsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<SupervisorDto>> GetAllSupervisorsAsync(GetAllSupervisorsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<PagedList<TeacherDto>> GetAllTeachersAsync(GetAllTeachersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<StudentDto>> GetChildrenByParentAsync(GetChildrenByParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemberProfileDto> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemberStatisticsDto> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ParentDto> RegisterParentAsync(RegisterParentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<StudentDto> RegisterStudentAsync(RegisterStudentRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<SearchMembersResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemberDto> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ParentDto> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<StudentDto> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<TeacherDto> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
