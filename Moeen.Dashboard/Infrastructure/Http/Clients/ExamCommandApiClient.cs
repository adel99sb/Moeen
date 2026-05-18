using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class ExamCommandApiClient
    {
        private readonly HttpClient _http;
        public ExamCommandApiClient(HttpClient http)
        {
            _http = http;
        }

        // 1. تسجيل اختبار جديد
        public async Task<GeneralResponse> RegisterExam(RegisterExamRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterExamRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 2. إضافة ملاحظات وتوصيات للامتحان
        public async Task<GeneralResponse> AddExamFeedback(AddExamFeedbackRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AddFeedbackOnExamRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 3. تحديث بيانات الامتحان (التاريخ والملاحظات)
        public async Task<GeneralResponse> UpdateExamInfo(UpdateExamInfoRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateInfoExamRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 4. تحديث نتيجة الامتحان (العلامة)
        public async Task<GeneralResponse> UpdateExamResult(UpdateExamResultRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdateResultExamRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 5. حذف نتيجة امتحان
        public async Task<GeneralResponse> DeleteExamResult(DeleteExamResultRequest request)
        {
            // نرسل الـ Request بـ جسم الطلب أو الرووت حسب تصميم الباك إند
            var response = await _http.PostAsJsonAsync(ApiRoutes.DeleteResultExamRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
