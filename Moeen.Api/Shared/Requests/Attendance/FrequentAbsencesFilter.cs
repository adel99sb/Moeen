using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Attendance
{
    public class FrequentAbsencesFilter
    {
        [Range(1, 100)]
        public int Threshold { get; set; } = 3;

        public Guid? CircleId { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}