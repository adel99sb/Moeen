using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Application.Services.Abstractions
{
    public interface IEnrollmentService
    {
        Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request);
        Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request);
        Task<GeneralResponse> AddSupervisorAsync(AddSupervisorRequest request);
        Task<GeneralResponse> PromoteTeacherToSupervisorAsync(Guid teacherId);
        Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request);
        Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request);
        Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request);
        Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request);
        Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request);
        Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request);
        Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request);
        Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request);
        Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request);
        Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request);
        Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request);
        Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request);
        Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request);
        Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId);
        Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request);
        Task<GeneralResponse> DeleteStudentAsync(Guid studentId);
        Task<GeneralResponse> DeleteTeacherAsync(Guid teacherId);
        Task<GeneralResponse> DeleteParentAsync(Guid parentId);
        Task<GeneralResponse> DeleteSupervisorAsync(Guid supervisorId);
        Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request);
    }
}
