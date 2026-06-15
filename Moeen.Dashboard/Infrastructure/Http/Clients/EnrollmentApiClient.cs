using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class EnrollmentApiClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<EnrollmentApiClient> _logger;

        public EnrollmentApiClient(HttpClient http, ILogger<EnrollmentApiClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterStudentAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "RegisterStudent");
        }

        public async Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AddTeacherAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "AddTeacher");
        }

        public async Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterParentAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "RegisterParent");
        }

        public async Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateMemberInfoAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateMemberInfo");
        }

        public async Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateStudentInfoAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateStudentInfo");
        }

        public async Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateTeacherInfoAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateTeacherInfo");
        }

        public async Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateParentInfoAsyncrRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateParentInfo");
        }

        public async Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.CancelMembershipAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "CancelMembership");
        }

        public async Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchMembersAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "SearchMembers");
        }

        public async Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetMemberProfileAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "GetMemberProfile");
        }

        public async Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateMemberStatusAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "UpdateMemberStatus");
        }

        public async Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Name", request.Name },
                { "MosqueId", request.MosqueId?.ToString() },
                { "Status", request.Status?.ToString() },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllStudentsAsyncRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllStudents");
        }

        public async Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Name", request.Name },
                { "MosqueId", request.MosqueId?.ToString() },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllTeachersAsyncRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllTeachers");
        }

        public async Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Name", request.Name },
                { "Phone", request.Phone },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllParentsAsyncRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllParents");
        }

        public async Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Name", request.Name },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllSupervisorsAsyncRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetAllSupervisors");
        }

        public async Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId)
        {
            var url = ApiRoutes.GetChildrenByParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetChildrenByParent");
        }

        public async Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "MosqueId", request.MosqueId?.ToString() },
                { "Status", request.Status?.ToString() },
                { "FromDate", request.FromDate?.ToString("o") },
                { "ToDate", request.ToDate?.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetMemberStatisticsAsyncRoute, query);
            var response = await _http.GetAsync(url);
            return await ReadGeneralResponseAsync(response, "GetMemberStatistics");
        }

        public async Task<GeneralResponse> DeleteStudentAsync(Guid studentId)
        {
            var url = ApiRoutes.DeleteStudentAsyncRoute.Replace("{studentId}", studentId.ToString());
            var response = await _http.DeleteAsync(url);
            return await ReadGeneralResponseAsync(response, "DeleteStudent");
        }

        public async Task<GeneralResponse> DeleteTeacherAsync(Guid teacherId)
        {
            var url = ApiRoutes.DeleteTeacherAsyncRoute.Replace("{teacherId}", teacherId.ToString());
            var response = await _http.DeleteAsync(url);
            return await ReadGeneralResponseAsync(response, "DeleteTeacher");
        }

        public async Task<GeneralResponse> DeleteParentAsync(Guid parentId)
        {
            var url = ApiRoutes.DeleteParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            var response = await _http.DeleteAsync(url);
            return await ReadGeneralResponseAsync(response, "DeleteParent");
        }

        public async Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.ExportMembersListAsyncRoute, request);
            return await ReadGeneralResponseAsync(response, "ExportMembersList");
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (string.IsNullOrWhiteSpace(body))
            {
                return response.IsSuccessStatusCode
                    ? GeneralResponse.Ok("تمت العملية بنجاح.")
                    : GeneralResponse.BadRequest($"فشل تنفيذ العملية. StatusCode={(int)response.StatusCode}");
            }

            try
            {
                var parsed = JsonSerializer.Deserialize<GeneralResponse>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (parsed != null)
                    return parsed;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(
                    ex,
                    "Enrollment API returned non-JSON response. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
                    operation,
                    (int)response.StatusCode,
                    contentType,
                    body.Length > 300 ? body[..300] : body);
            }

            var message = body.Length > 700 ? body[..700] : body;
            return new GeneralResponse(
                string.IsNullOrWhiteSpace(message) ? "فشل تنفيذ العملية." : message,
                success: false,
                statusCode: (int)response.StatusCode);
        }
    }
}