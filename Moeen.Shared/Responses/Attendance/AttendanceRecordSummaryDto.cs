using Moeen.Shared.Constants;
using System;

namespace Moeen.Shared.Responses.Attendance
{
    public class AttendanceRecordSummaryDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
    }
}