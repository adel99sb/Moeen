using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class Halqa
    {
        public Guid Id { get; set; }
        public Guid FoujId { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid? TeacherId { get; set; }

        [Column("type")]
        public string Type { get; set; } = string.Empty;

        public Teacher Teacher { get; set; } = null!;
        public Fouj Fouj { get; set; } = null!;
        public ICollection<ProgressEntry> ProgressEntries { get; set; } = new List<ProgressEntry>();
        public ICollection<HalqaSession> HalqeSessions { get; set; } = new List<HalqaSession>();
        public ICollection<ExamTeacherHalqa> ExamTeacherHalqas { get; set; } = new List<ExamTeacherHalqa>();
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
