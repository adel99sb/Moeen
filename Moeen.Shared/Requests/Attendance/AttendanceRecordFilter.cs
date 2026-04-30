using Moeen.Shared.Constants;
using System;

namespace Moeen.Shared.Requests.Attendance
{
    public class AttendanceRecordFilter
    {
        public Guid? StudentId { get; set; }
        public Guid? CircleId { get; set; }
        public AttendanceStatus? Status { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}