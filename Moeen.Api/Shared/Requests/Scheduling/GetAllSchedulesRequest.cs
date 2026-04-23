using System;

namespace Moeen.Api.Shared.Requests.Scheduling
{
    public class GetAllSchedulesRequest
    {
        public Guid? CircleId { get; set; }
        public Guid? TeacherId { get; set; }
        public string? DayOfWeek { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}