using System;

namespace Moeen.Api.Shared.Responses.SystemConfiguration
{
    public class TimePeriodDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsActive { get; set; }
    }
}