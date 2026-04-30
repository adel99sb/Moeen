namespace Moeen.Shared.Requests.ExamPhase
{
    public class GetExamPhasesRequest
    {
        // يمكن إضافة Pagination أو ترتيب إذا أردت
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "PhaseName";
        public bool SortDescending { get; set; } = false;
    }
}