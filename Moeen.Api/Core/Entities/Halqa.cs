using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Halqa
    {
        public Guid Id { get; set; }
        public Guid FoujId { get; set; }
        public string Name { get; set; }
        public Guid TeacherId { get; set; }

        [Column("type")]
        public string Type { get; set; }

        public Teacher Teacher { get; set; }
        public Fouj Fouj { get; set; }
        public ICollection<ProgressEntry> ProgressEntries { get; set; }
        public ICollection<HalqaSession> HalqeSessions { get; set; }
        public ICollection<ExamTeacherHalqa> ExamTeacherHalqas { get; set; }
    }
}
