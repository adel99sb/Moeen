using Moeen.Api.Core.Constants;
using System;

namespace Moeen.Api.Shared.Responses.Attendance
{
    public class AttendanceRecordDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public AttendanceStatus Status { get; set; }
        public string? Note { get; set; }
    }
}