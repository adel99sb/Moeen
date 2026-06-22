namespace Moeen.Api.Core.Entities
{
    public class PdfFile
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public Guid SaturdayLessonId { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string uploaded_by { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public Mosque Mosque { get; set; } = null!;
        public SaturdayLesson SaturdayLesson { get; set; } = null!;
    }
}
