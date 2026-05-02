using Moeen.Shared.Constants;

namespace Moeen.Shared.Requests.ExamGrading
{
    public class GetGradingCriteriaRequest
    {
        public ExamType? ApplicableTo { get; set; } // تصفية حسب نوع الاختبار (اختياري)
    }
}