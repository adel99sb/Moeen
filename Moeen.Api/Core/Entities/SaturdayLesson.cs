namespace Moeen.Api.Core.Entities
{
    public class SaturdayLesson
    {
        public Guid Id {  get; set; }
        public Guid SaturdayHalqeId { get; set; }
        public int lesson_number { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public SaturdayHalqa SaturdayHalqe { get; set; }
        public ICollection<Attendance> attendances { get; set; }
        public ICollection<PdfFile> PdfFiles { get; set; }

    }
}
