using System.Net.Http.Json;
using Moeen.Shared.Requests.Goal;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class GoalApiClient
    {
        private readonly HttpClient _httpClient;

        public GoalApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> RecordDailyEntryAsync(RecordDailyEntryRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(ApiRoutes.RecordDailyEntryAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                   ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر أثناء حفظ البيانات اليومية.");
        }

        public async Task<GeneralResponse> UpdateProgressRecordAsync(UpdateProgressRecordRequest request)
        {
            var url = ApiRoutes.UpdateProgressRecordAsyncRoute.Replace("{recordId}", request.RecordId.ToString());
            var response = await _httpClient.PutAsJsonAsync(url, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                   ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر أثناء تحديث السجل.");
        }

        public async Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request)
        {
            var url = ApiRoutes.GetStudentProgressSummaryAsyncRoute.Replace("{studentId}", request.StudentId.ToString());
            var query = new List<string>();
            if (request.FromDate.HasValue) query.Add($"fromDate={request.FromDate.Value:yyyy-MM-dd}");
            if (request.ToDate.HasValue) query.Add($"toDate={request.ToDate.Value:yyyy-MM-dd}");
            if (query.Count > 0) url += "?" + string.Join("&", query);

            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع ملخص تقدم الطالب.");
        }

        public async Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request)
        {
            var url = ApiRoutes.GetStudentProgressHistoryAsyncRoute.Replace("{studentId}", request.StudentId.ToString());
            var query = new List<string>
            {
                $"pageNumber={Math.Max(1, request.PageNumber)}",
                $"pageSize={Math.Max(1, request.PageSize)}"
            };

            if (request.FromDate.HasValue) query.Add($"fromDate={request.FromDate.Value:yyyy-MM-dd}");
            if (request.ToDate.HasValue) query.Add($"toDate={request.ToDate.Value:yyyy-MM-dd}");
            if (request.RecordType.HasValue) query.Add($"recordType={(int)request.RecordType.Value}");

            url += "?" + string.Join("&", query);
            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع السجل التفصيلي للطالب.");
        }

        public async Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetHalqaPerformanceOverviewRequest request)
        {
            var url = ApiRoutes.GetCirclePerformanceOverviewAsyncRoute;
            var query = new List<string>();
            if (request.HalqaId.HasValue) query.Add($"halqaId={request.HalqaId.Value}");
            if (request.FromDate.HasValue) query.Add($"fromDate={request.FromDate.Value:yyyy-MM-dd}");
            if (request.ToDate.HasValue) query.Add($"toDate={request.ToDate.Value:yyyy-MM-dd}");
            if (query.Count > 0) url += "?" + string.Join("&", query);

            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع لوحة أداء الحلقة.");
        }
    }
}
