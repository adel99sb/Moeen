using Moeen.App.Infrastructure.Http.Clients;
using Moeen.App.Services.Abstractions;
using Moeen.Shared.Responses.Enrollment;
using System.Text.Json;

namespace Moeen.App.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly EnrollmentApiClient _apiClient;

        public EnrollmentService(EnrollmentApiClient apiClient)
        {
            _apiClient = apiClient;
        }

        public async Task<MemberProfileDto?> GetMemberProfileAsync(string memberId)
        {
            var res = await _apiClient.GetMemberProfileAsync(memberId);
            if (res == null || !res.Success)
                return null;

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<MemberProfileDto>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data;
        }

        public async Task<List<StudentDto>> GetChildrenByParentAsync(Guid parentId)
        {
            var res = await _apiClient.GetChildrenByParentAsync(parentId);
            if (res == null || !res.Success)
                return new List<StudentDto>();

            var json = JsonSerializer.Serialize(res.Data);
            var data = JsonSerializer.Deserialize<List<StudentDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return data ?? new List<StudentDto>();
        }
    }
}
