using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class ExamHalqaService : IExamHalqaService
    {
        private readonly ExamHalqaApiClient _client;

        public ExamHalqaService(ExamHalqaApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> CreateExamTeacherAsync(CreateExamTeacherRequest request)
        {
            var res = await _client.CreateExamTeacher(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تسجيل أستاذ الاختبارات");
            return res;
        }

        public async Task<GeneralResponse> GetExamTeacherByIdAsync(GetExamTeacherByIdRequest request)
        {
            var res = await _client.GetExamTeacherById(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية جلب بيانات أستاذ الاختبارات");
            return res;
        }

        public async Task<GeneralResponse> AssignHalqaToExamTeacherAsync(AssignHalqaToExamTeacherRequest request)
        {
            var res = await _client.AssignHalqaToExamTeacher(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تعيين الحلقة لأستاذ الاختبارات");
            return res;
        }
    }
}
