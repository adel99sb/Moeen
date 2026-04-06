using Moeen.Api.Core.Constants;

namespace Moeen.Api.Shared.Requests.ExamGrading
{
    public class GetGradingCriteriaRequest
    {
        public ExamType? ApplicableTo { get; set; } // تصفية حسب نوع الاختبار (اختياري)
    }
}