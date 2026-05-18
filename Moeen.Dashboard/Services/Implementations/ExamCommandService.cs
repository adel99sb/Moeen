using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class ExamCommandService : IExamCommandService
    {
        private readonly ExamCommandClient _client;

        public ExamCommandService(ExamCommandClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> RegisterExamAsync(RegisterExamRequest request)
        {
            var res = await _client.RegisterExam(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تسجيل الاختبار");
            return res;
        }

        public async Task<GeneralResponse> AddExamFeedbackAsync(AddExamFeedbackRequest request)
        {
            var res = await _client.AddExamFeedback(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية إضافة ملاحظات الاختبار");
            return res;
        }

        public async Task<GeneralResponse> UpdateExamInfoAsync(UpdateExamInfoRequest request)
        {
            var res = await _client.UpdateExamInfo(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تحديث بيانات الاختبار");
            return res;
        }

        public async Task<GeneralResponse> UpdateExamResultAsync(UpdateExamResultRequest request)
        {
            var res = await _client.UpdateExamResult(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تحديث نتيجة الاختبار");
            return res;
        }

        public async Task<GeneralResponse> DeleteExamResultAsync(DeleteExamResultRequest request)
        {
            var res = await _client.DeleteExamResult(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية حذف نتيجة الاختبار");
            return res;
        }
    }
}