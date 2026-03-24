namespace Moeen.Api.Shared.Requests.SystemConfiguration
{
    public class GetSystemLogsRequest
    {
        // يمكن إضافة Pagination أو ترشيح لاحقاً
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public string Level { get; set; } // اختياري
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}