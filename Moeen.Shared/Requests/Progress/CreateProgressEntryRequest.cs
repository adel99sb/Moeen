namespace Moeen.Shared.Requests.Progress
{
    public class CreateProgressEntryRequest
    {
        public Guid StudentId { get; set; }
        public Guid TeacherId { get; set; }
        public int JuzNumber { get; set; }
        public int PageNumber { get; set; }
        public DateTime? Date { get; set; }
    }
}