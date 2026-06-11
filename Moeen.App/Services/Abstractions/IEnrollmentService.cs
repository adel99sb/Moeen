using Moeen.Shared.Responses.Enrollment;

namespace Moeen.App.Services.Abstractions
{
    public interface IEnrollmentService
    {
        Task<MemberProfileDto?> GetMemberProfileAsync(string memberId);
        Task<List<StudentDto>> GetChildrenByParentAsync(Guid parentId);
    }
}
