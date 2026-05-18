using System.Net.Http.Json;
using Moeen.Shared.Requests.ExamPhase;
using Moeen.Shared.Responses.ExamPhase;
using Microsoft.AspNetCore.WebUtilities;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class ExamPhaseApiClient
    {
        private readonly HttpClient _http;

        public ExamPhaseApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<ExamPhaseDto> DefineExamPhaseAsync(DefineExamPhaseRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.DefineExamPhaseAsyncRoute, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ExamPhaseDto>() ?? new ExamPhaseDto();
        }

        public async Task<GetExamPhasesResponse> GetExamPhasesAsync(GetExamPhasesRequest request)
        {
            var query = new Dictionary<string, string?>
            {
                { "pageNumber", request.PageNumber.ToString() },
                { "pageSize", request.PageSize.ToString() },
                { "sortBy", request.SortBy },
                { "sortDescending", request.SortDescending.ToString().ToLower() }
            };
            var url = QueryHelpers.AddQueryString(ApiRoutes.GetAllExamPhasesAsyncRoute, query);
            return await _http.GetFromJsonAsync<GetExamPhasesResponse>(url) ?? new GetExamPhasesResponse();
        }

        public async Task<ExamPhaseDto> GetExamPhaseByIdAsync(GetExamPhaseByIdRequest request)
        {
            var url = ApiRoutes.GetExamPhaseByIdAsyncRoute.Replace("{phaseId}", request.PhaseId.ToString());
            return await _http.GetFromJsonAsync<ExamPhaseDto>(url) ?? new ExamPhaseDto();
        }

        public async Task<List<ExamPhaseDto>> GetExamPhasesByCircleAsync(GetExamPhasesByCircleRequest request)
        {
            var url = ApiRoutes.GetExamPhasesByCircleAsyncRoute.Replace("{circleId}", request.CircleId.ToString());
            return await _http.GetFromJsonAsync<List<ExamPhaseDto>>(url) ?? new List<ExamPhaseDto>();
        }

        public async Task<ExamPhaseDto> UpdateExamPhaseInfoAsync(UpdateExamPhaseInfoRequest request)
        {
            // نمرر الـ PhaseId بالرابط والـ Request بجسم الطلب
            var url = ApiRoutes.UpdateExamPhaseInfoAsyncRoute.Replace("{phaseId}", request.PhaseId.ToString());
            var response = await _http.PutAsJsonAsync(url, request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ExamPhaseDto>() ?? new ExamPhaseDto();
        }

        public async Task<DeleteExamPhaseResponse> DeleteExamPhaseAsync(DeleteExamPhaseRequest request)
        {
            var url = ApiRoutes.DeleteExamPhaseAsyncRoute.Replace("{phaseId}", request.PhaseId.ToString());
            var response = await _http.DeleteAsync(url);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<DeleteExamPhaseResponse>() ?? new DeleteExamPhaseResponse();
        }
    }
}