using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Moeen.Dashboard.Services.Abstractions;
using Microsoft.AspNetCore.WebUtilities;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class EnrollmentApiClient
    {
        private readonly HttpClient _http;
        private readonly ITokenService _tokenService;
        private readonly ILogger<EnrollmentApiClient> _logger;

        public EnrollmentApiClient(HttpClient http, ITokenService tokenService, ILogger<EnrollmentApiClient> logger)
        {
            _http = http;
            _tokenService = tokenService;
            _logger = logger;
        }

        public async Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.RegisterStudentAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "RegisterStudent");
        }

        public async Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.AddTeacherAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "AddTeacher");
        }

        public async Task<GeneralResponse> AddSupervisorAsync(AddSupervisorRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.AddSupervisorAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "AddSupervisor");
        }

        public async Task<GeneralResponse> PromoteTeacherToSupervisorAsync(Guid teacherId)
        {
            var url = ApiRoutes.PromoteTeacherToSupervisorAsyncRoute.Replace("{teacherId}", teacherId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Post, url);
            return await ReadGeneralResponseAsync(response, "PromoteTeacherToSupervisor");
        }

        public async Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.RegisterParentAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "RegisterParent");
        }

        public async Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateMemberInfoAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "UpdateMemberInfo");
        }

        public async Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateStudentInfoAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "UpdateStudentInfo");
        }

        public async Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateTeacherInfoAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "UpdateTeacherInfo");
        }

        public async Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateParentInfoAsyncrRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "UpdateParentInfo");
        }

        public async Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.CancelMembershipAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "CancelMembership");
        }

        public async Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.SearchMembersAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "SearchMembers");
        }

        public async Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.GetMemberProfileAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "GetMemberProfile");
        }

        public async Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Put, ApiRoutes.UpdateMemberStatusAsyncRoute, JsonContent.Create(request));
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
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
            return await ReadGeneralResponseAsync(response, "GetAllStudents");
        }

        public async Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "Name", request.Name },
                { "MosqueId", request.MosqueId?.ToString() },
                { "Status", request.Status?.ToString() },
                { "PageNumber", request.PageNumber.ToString() },
                { "PageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllTeachersAsyncRoute, query);
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
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
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
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
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
            return await ReadGeneralResponseAsync(response, "GetAllSupervisors");
        }

        public async Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId)
        {
            var url = ApiRoutes.GetChildrenByParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
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
            using var response = await SendAuthorizedAsync(HttpMethod.Get, url);
            return await ReadGeneralResponseAsync(response, "GetMemberStatistics");
        }

        public async Task<GeneralResponse> DeleteStudentAsync(Guid studentId)
        {
            var url = ApiRoutes.DeleteStudentAsyncRoute.Replace("{studentId}", studentId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Delete, url);
            return await ReadGeneralResponseAsync(response, "DeleteStudent");
        }

        public async Task<GeneralResponse> DeleteTeacherAsync(Guid teacherId)
        {
            var url = ApiRoutes.DeleteTeacherAsyncRoute.Replace("{teacherId}", teacherId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Delete, url);
            return await ReadGeneralResponseAsync(response, "DeleteTeacher");
        }

        public async Task<GeneralResponse> DeleteParentAsync(Guid parentId)
        {
            var url = ApiRoutes.DeleteParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Delete, url);
            return await ReadGeneralResponseAsync(response, "DeleteParent");
        }

        public async Task<GeneralResponse> DeleteSupervisorAsync(Guid supervisorId)
        {
            var url = ApiRoutes.DeleteSupervisorAsyncRoute.Replace("{supervisorId}", supervisorId.ToString());
            using var response = await SendAuthorizedAsync(HttpMethod.Delete, url);
            return await ReadGeneralResponseAsync(response, "DeleteSupervisor");
        }

        public async Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
        {
            using var response = await SendAuthorizedAsync(HttpMethod.Post, ApiRoutes.ExportMembersListAsyncRoute, JsonContent.Create(request));
            return await ReadGeneralResponseAsync(response, "ExportMembersList");
        }

        private async Task<HttpResponseMessage> SendAuthorizedAsync(HttpMethod method, string route, HttpContent? content = null)
        {
            var token = await _tokenService.Get();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.");

            using var message = new HttpRequestMessage(method, route)
            {
                Content = content
            };
            message.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return await _http.SendAsync(message);
        }

        private async Task<GeneralResponse> ReadGeneralResponseAsync(HttpResponseMessage response, string operation)
        {
            var body = await response.Content.ReadAsStringAsync();
            var contentType = response.Content.Headers.ContentType?.MediaType;

            if (!response.IsSuccessStatusCode)
            {
                var errorMessage = TryReadApiErrorMessage(body);
                var bodyStart = string.IsNullOrWhiteSpace(body) ? string.Empty : body.Length > 300 ? body[..300] : body;

                _logger.LogWarning(
                    "Enrollment API request failed. Operation={Operation}, StatusCode={StatusCode}, ContentType={ContentType}, BodyStart={BodyStart}",
                    operation,
                    (int)response.StatusCode,
                    contentType,
                    bodyStart);

                return new GeneralResponse(
                    string.IsNullOrWhiteSpace(errorMessage) ? $"فشل تنفيذ العملية. StatusCode={(int)response.StatusCode}" : errorMessage,
                    success: false,
                    statusCode: (int)response.StatusCode);
            }

            if (string.IsNullOrWhiteSpace(body))
            {
                return GeneralResponse.Ok("تمت العملية بنجاح.");
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

        private static string? TryReadApiErrorMessage(string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.TryGetProperty("message", out var messageElement) &&
                    messageElement.ValueKind == JsonValueKind.String)
                {
                    var message = messageElement.GetString();
                    if (!string.IsNullOrWhiteSpace(message))
                        return message;
                }

                if (root.TryGetProperty("title", out var titleElement) &&
                    titleElement.ValueKind == JsonValueKind.String)
                {
                    var title = titleElement.GetString();
                    if (!string.IsNullOrWhiteSpace(title))
                        return title;
                }
            }
            catch (JsonException)
            {
                return body.Length > 700 ? body[..700] : body;
            }

            return body.Length > 700 ? body[..700] : body;
        }
    }
}
