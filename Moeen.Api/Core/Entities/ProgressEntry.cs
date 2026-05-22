using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class ProgressEntry
    {
        //internal object studentId;

        public Guid Id { get; set; }

        [Column("studentId")]
        public Guid StudentId { get; set; }

        public Guid HalqaId { get; set; }
        public Guid TeacherId { get; set; }

        [Column("juz_number")]
        public int JuzNumber { get; set; }

        [Column("page_number")]
        public int PageNumber { get; set; }

        [Column("memorized_until")]
        public int MemorizedUntil { get; set; }

        [Column("next_target")]
        public int NextTarget { get; set; }

        [Column("level_score")]
        public int LevelScore { get; set; }

        [Column("date")]
        public DateTime Date { get; set; }

        [Column("is_deleted")]
        public bool IsDeleted { get; set; }

        [Column("deleted_at")]
        public DateTime? DeletedAt { get; set; }

        public Student Student { get; set; }
        public Halqa Halqa { get; set; }
        public Teacher Teacher { get; set; }
    }
}
