using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.SystemConfiguration
{
    public class GetSystemLogsResponse
    {
        public List<SystemLogDto> Logs { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}