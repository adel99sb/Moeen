using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class ExamHalqaApiClient
    {
        private readonly HttpClient _http;
        public ExamHalqaApiClient(HttpClient http)
        {
            _http = http;
        }

        // 1. تسجيل أستاذ اختبارات جديد
        public async Task<GeneralResponse> CreateExamTeacher(CreateExamTeacherRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.CreateExamTeacherRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 2. جلب أستاذ اختبارات بواسطة الـ ID
        public async Task<GeneralResponse> GetExamTeacherById(GetExamTeacherByIdRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetExamTeacherByIdRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 3. تعيين حلقة لأستاذ الاختبارات
        public async Task<GeneralResponse> AssignHalqaToExamTeacher(AssignHalqaToExamTeacherRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AssignHalqaToExamTeacherRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
