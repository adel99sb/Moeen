using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
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
            // بناءً على الـ Route في الباك إند: records/{recordId:guid}
            var response = await _httpClient.PutAsJsonAsync(ApiRoutes.UpdateProgressRecordAsyncRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                   ?? GeneralResponse.BadRequest("فشل الاتصال بالسيرفر أثناء تحديث السجل.");
        }

        public async Task<GeneralResponse> GetStudentProgressSummaryAsync(GetStudentProgressSummaryRequest request)
        {
            // تحويل الـ FromQuery إلى Query Strings وتمرير الـ StudentId في الـ Route
            var url = ApiRoutes.GetStudentProgressSummaryAsyncRoute.Replace("{studentId}", request.StudentId.ToString());

            var query = "";
            if (request.FromDate.HasValue) query += $"fromDate={request.FromDate.Value:yyyy-MM-dd}&";
            if (request.ToDate.HasValue) query += $"toDate={request.ToDate.Value:yyyy-MM-dd}&";

            if (!string.IsNullOrEmpty(query)) url += "?" + query.TrimEnd('&');

            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع ملخص تقدم الطالب.");
        }

        public async Task<GeneralResponse> GetStudentProgressHistoryAsync(GetStudentProgressHistoryRequest request)
        {
            // تركيب الرابط للتاريخ، الترقيم ونوع السجل المطلوب
            var url = ApiRoutes.GetStudentProgressHistoryAsyncRoute.Replace("{studentId}", request.StudentId.ToString())
                                                                    .Replace("{pageNumber}", request.PageNumber.ToString())
                                                                    .Replace("{pageSize}", request.PageSize.ToString());    

            if (request.FromDate.HasValue) url += $"&fromDate={request.FromDate.Value:yyyy-MM-dd}";
            if (request.ToDate.HasValue) url += $"&toDate={request.ToDate.Value:yyyy-MM-dd}";
            if (request.RecordType.HasValue) url += $"&recordType={(int)request.RecordType.Value}";

            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع السجل التفصيلي للطالب.");
        }

        public async Task<GeneralResponse> GetCirclePerformanceOverviewAsync(GetCirclePerformanceOverviewRequest request)
        {
            var url = ApiRoutes.GetCirclePerformanceOverviewAsyncRoute;

            var query = "";
            if (request.CircleId.HasValue) query += $"circleId={request.CircleId.Value}&";
            if (request.FromDate.HasValue) query += $"fromDate={request.FromDate.Value:yyyy-MM-dd}&";
            if (request.ToDate.HasValue) query += $"toDate={request.ToDate.Value:yyyy-MM-dd}&";

            if (!string.IsNullOrEmpty(query)) url += "?" + query.TrimEnd('&');

            return await _httpClient.GetFromJsonAsync<GeneralResponse>(url)
                   ?? GeneralResponse.BadRequest("فشل استرجاع لوحة أداء الحلقة.");
        }
    }
}
