namespace Moeen.Shared.Responses.Progress
{
    public class ProgressEntryResponse
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentFullName { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherFullName { get; set; }
        public int JuzNumber { get; set; }
        public int PageNumber { get; set; }
        public DateTime Date { get; set; }
    }
}