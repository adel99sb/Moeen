using System.Net.Http.Json;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Microsoft.AspNetCore.WebUtilities; // للتعامل مع الـ Query Parameters بسهولة

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class EnrollmentApiClient
    {
        private readonly HttpClient _http;

        public EnrollmentApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterStudentAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AddTeacherAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterParentAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateMemberInfoAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateStudentInfoAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateTeacherInfoAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateParentInfoAsyncrRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.CancelMembershipAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchMembersAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetMemberProfileAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }
          public async Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateMemberStatusAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        // --- ميثودات الـ GET مع الـ Query String ---

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
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
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
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
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
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
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
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetChildrenByParentAsync(Guid parentId)
        {
            var url = ApiRoutes.GetChildrenByParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            return await _http.GetFromJsonAsync<GeneralResponse>(url) 
                   ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "MosqueId", request.MosqueId?.ToString() },
                { "Status", request.Status?.ToString() },
                { "FromDate", request.FromDate?.ToString("o") }, // صيغة ISO للتاريخ
                { "ToDate", request.ToDate?.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetMemberStatisticsAsyncRoute, query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        // --- ميثودات الـ DELETE ---
            public async Task<GeneralResponse> DeleteStudentAsync(Guid studentId)
        {
            var url = ApiRoutes.DeleteStudentAsyncRoute.Replace("{studentId}", studentId.ToString());
            var response = await _http.DeleteAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> DeleteTeacherAsync(Guid teacherId)
        {
            var url = ApiRoutes.DeleteTeacherAsyncRoute.Replace("{teacherId}", teacherId.ToString());
            var response = await _http.DeleteAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> DeleteParentAsync(Guid parentId)
        {
            var url = ApiRoutes.DeleteParentAsyncRoute.Replace("{parentId}", parentId.ToString());
            var response = await _http.DeleteAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.ExportMembersListAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }
    }
}
    