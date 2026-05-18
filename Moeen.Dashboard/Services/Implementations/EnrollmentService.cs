using Moeen.Dashboard.Application.Services.Abstractions;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Application.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly EnrollmentApiClient _client;
        // هنا يمكنك حقن الـ BaseService الخاص بك إذا كنتِ تستخدمين ميثود CallAsync عامة من كلاس أب
        // سأفترض هنا استدعاء الميثود مباشرة أو من كلاس الخدمات العام لديكِ لحماية التطبيق.

        public EnrollmentService(EnrollmentApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
            => await _client.RegisterStudentAsync(request);

        public async Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
            => await _client.AddTeacherAsync(request);

        public async Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
            => await _client.RegisterParentAsync(request);

        public async Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
            => await _client.UpdateMemberInfoAsync(request);

        public async Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
            => await _client.UpdateStudentInfoAsync(request);

        public async Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
            => await _client.UpdateTeacherInfoAsync(request);

        public async Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
            => await _client.UpdateParentInfoAsync(request);

        public async Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
            => await _client.CancelMembershipAsync(request);

        public async Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
            => await _client.SearchMembersAsync(request);

        public async Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
            => await _client.GetMemberProfileAsync(request);

        public async Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
            => await _client.UpdateMemberStatusAsync(request);

        public async Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request)
            => await _client.GetAllStudentsAsync(request);

        public async Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request)
            => await _client.GetAllTeachersAsync(request);

        public async Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request)
            => await _client.GetAllParentsAsync(request);

        public async Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request)
            => await _client.GetAllSupervisorsAsync(request);

        public async Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId)
            => await _client.GetChildrenByParentAsync(parentId);

        public async Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
            => await _client.GetMemberStatisticsAsync(request);

        public async Task<GeneralResponse> DeleteStudentAsync(Guid studentId)
            => await _client.DeleteStudentAsync(studentId);

        public async Task<GeneralResponse> DeleteTeacherAsync(Guid teacherId)
            => await _client.DeleteTeacherAsync(teacherId);

        public async Task<GeneralResponse> DeleteParentAsync(Guid parentId)
            => await _client.DeleteParentAsync(parentId);

        public async Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
            => await _client.ExportMembersListAsync(request);
    }
}
