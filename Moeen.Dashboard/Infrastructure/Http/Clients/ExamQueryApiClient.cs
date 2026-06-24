 using System.Net.Http.Json;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
using Microsoft.AspNetCore.WebUtilities;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class ExamQueryApiClient
    {
        private readonly HttpClient _http;

        public ExamQueryApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            // اعتمدنا الـ POST المباشر والنظيف
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetExamResultByIdAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchExamResultsAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetStudentExamsAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate?.ToString("o") },
                { "toDate", request.ToDate?.ToString("o") },
                { "pageNumber", request.PageNumber.ToString() },
                { "pageSize", request.PageSize.ToString() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetExamsByHalqaAsyncRoute.Replace("{halqaId}", request.HalqaId.ToString()), query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate.ToString("o") },
                { "toDate", request.ToDate.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetStudentExamsByDateRangeAsyncRoute.Replace("{studentId}", request.StudentId.ToString()), query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate?.ToString("o") },
                { "toDate", request.ToDate?.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetExamsByTeacherAsyncRoute.Replace("{teacherId}", request.TeacherId.ToString()), query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate?.ToString("o") },
                { "toDate", request.ToDate?.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetExamsByPhaseAsyncRoute.Replace("{phaseId}", request.PhaseId.ToString()), query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }
        public async Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate?.ToString("o") },
                { "toDate", request.ToDate?.ToString("o") }
            };

            var url = QueryHelpers.AddQueryString(ApiRoutes.GetExamStatisticsAsyncRoute, query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "fromDate", request.FromDate?.ToString("o") },
                { "toDate", request.ToDate?.ToString("o") }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetHalqaExamAnalyticsAsyncRoute.Replace("{HalqaId}", request.HalqaId.ToString()), query);
            return await _http.GetFromJsonAsync<GeneralResponse>(url) ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.CompareHalqasPerformanceAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }

        public async Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.PrepareExamDataForExportAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>() ?? GeneralResponse.BadRequest("خطأ في الاتصال");
        }
    }
}