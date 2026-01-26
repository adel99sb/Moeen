namespace Moeen.Api.Core.Entities
{
    public class ProgressEntry
    {
        public Guid Id {  get; set; }
        public Guid studentId { get; set; }
        public Guid HalqaId { get; set; }
        public Guid TeacherId { get; set; }
        public int juz_number { get; set; }
        public int page_number { get; set; }
        public int memorized_until { get; set; }
        public int next_target { get; set; }
        public int level_score { get; set; }
        public DateTime date { get; set; }
        public Student Student { get; set; }
        public Halqa Halqa { get; set; }
        public Teacher Teacher { get; set; }


    }
}
