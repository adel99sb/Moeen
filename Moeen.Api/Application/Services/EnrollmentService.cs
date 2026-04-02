using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Enrollment;
using Moeen.Api.Shared.Responses.Enrollment;

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

        public Task<byte[]> ExportMembersListAsync(ExportMembersRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MemberProfileDto> GetMemberProfileAsync(GetMemberProfileRequest request)
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
    }
}
