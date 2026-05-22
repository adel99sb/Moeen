

namespace Moeen.Shared.Responses.Attendance
{
    public class StudentAbsenceReportDto
    {
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public int TotalExpectedSessions { get; set; }    // عدد الجلسات المتوقعة في النطاق (حسب HalqaSessions)
        public int PresentDays { get; set; }              // حاضر / متأخر
        public int ExcusedAbsences { get; set; }          // بعذر
        public int UnexcusedAbsences { get; set; }        // غياب بدون عذر (يتضمن غياب عن جلسة متوقعة بدون سجل حضور)
        public int TotalAbsenceDays { get; set; }         // Excused + Unexcused

        public int LongestConsecutiveAbsences { get; set; }

        public List<string> Alerts { get; set; } = new List<string>();

        // (اختياري) تواريخ الغياب لتفريغ سريع في الواجهة
        public List<DateTime> AbsentDates { get; set; } = new List<DateTime>();
    }
}