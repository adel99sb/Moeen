using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.ExamCommand;
using Moeen.Shared.Responses.ExamCommand;

namespace Moeen.Api.Application.Services
{
    public class ExamCommandService : IExamCommandService
    {
        private readonly IUnitOfWork _unitOfWork;
        // ✅ Constructor Dependency Injection
        public ExamCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<ExamFeedbackDto> AddExamFeedbackAsync(AddExamFeedbackRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            // 1. جلب الامتحان والتأكد من وجوده
            var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(request.ExamId);
            if (exam == null)
                throw new ArgumentException("الامتحان غير موجود.", nameof(request.ExamId));

            // 2. تحديث البيانات حسب الـ Schema الحالي
            // ملاحظة: جدول Exam يحتوي حالياً على notes فقط، لذا نربط Feedback به مباشرة.
            // Recommendations غير موجود كحقل في الجدول حالياً، لذا يُعاد في الـ DTO كما هو لحين تحديث المخطط مستقبلاً.
            exam.notes = request.Feedback;

            // 3. الحفظ
            await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
            await _unitOfWork.CompleteAsync();

            // 4. إرجاع الاستجابة موحّدة ومطابقة لـ ExamFeedbackDto
            return new ExamFeedbackDto
            {
                ExamId = exam.Id,
                Feedback = exam.notes,
                Recommendations = request.Recommendations,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public Task<DeleteExamResultResponse> DeleteExamResultAsync(DeleteExamResultRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> RegisterExamAsync(RegisterExamRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> UpdateExamInfoAsync(UpdateExamInfoRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ExamResultDto> UpdateExamResultAsync(UpdateExamResultRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
