using System.Collections.Generic;

namespace Moeen.Shared.Responses.SystemConfiguration
{
    public class GetSystemLogsResponse
    {
        public List<SystemLogDto> Logs { get; set; } = new List<SystemLogDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}