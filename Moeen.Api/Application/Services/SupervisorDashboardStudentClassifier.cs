using Moeen.Shared.Responses.SupervisorDashboard;

namespace Moeen.Api.Application.Services
{
    public sealed record SupervisorDashboardStudentPerformance(
        Guid StudentId,
        string StudentName,
        int CurrentScore,
        double AverageAssessmentScore,
        int AssessmentCount);

    public static class SupervisorDashboardStudentClassifier
    {
        public static List<DashboardStudentDto> BuildExcellentStudents(
            IEnumerable<SupervisorDashboardStudentPerformance> performances,
            int take = 5)
        {
            return performances
                .Where(HasAssessmentActivity)
                .OrderByDescending(p => p.AverageAssessmentScore)
                .ThenByDescending(p => p.AssessmentCount)
                .ThenBy(p => p.StudentName)
                .Take(Math.Max(0, take))
                .Select(p => new DashboardStudentDto
                {
                    StudentId = p.StudentId,
                    StudentName = p.StudentName,
                    Score = (int)Math.Round(p.AverageAssessmentScore),
                    Badge = p.AverageAssessmentScore >= 90 ? "ممتاز" : "متميز",
                    Reason = "من أعلى الطلاب نتائجاً في التسميعات والاختبارات خلال الفترة الحالية"
                })
                .ToList();
        }

        public static List<DashboardStudentDto> BuildStrugglingStudents(
            IEnumerable<SupervisorDashboardStudentPerformance> performances,
            int take = 5)
        {
            return performances
                .Where(HasAssessmentActivity)
                .OrderBy(p => p.AverageAssessmentScore)
                .ThenBy(p => p.AssessmentCount)
                .ThenBy(p => p.StudentName)
                .Take(Math.Max(0, take))
                .Select(p => new DashboardStudentDto
                {
                    StudentId = p.StudentId,
                    StudentName = p.StudentName,
                    Score = (int)Math.Round(p.AverageAssessmentScore),
                    Badge = p.AverageAssessmentScore < 60 ? "متابعة عاجلة" : "متابعة خطة",
                    Reason = "من أدنى الطلاب نتائجاً في التسميعات والاختبارات خلال الفترة الحالية"
                })
                .ToList();
        }

        private static bool HasAssessmentActivity(SupervisorDashboardStudentPerformance performance)
            => performance.AssessmentCount > 0;
    }
}
