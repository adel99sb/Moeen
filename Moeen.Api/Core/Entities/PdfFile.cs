namespace Moeen.Api.Core.Entities
{
    public class PdfFile
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public Guid SaturdayLessonId { get; set; }
        public string FileUrl { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string uploaded_by { get; set; }
        public DateTime created_at { get; set; }
        public Mosque Mosque { get; set; }
        public SaturdayLesson SaturdayLesson { get; set; }
    }
}
