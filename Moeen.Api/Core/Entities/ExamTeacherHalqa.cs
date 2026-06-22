namespace Moeen.Api.Core.Entities
{
    public class ExamTeacherHalqa
    {
        public Guid Id { get; set; }

        // Foreign Keys
        public Guid ExamTeacherId { get; set; }  // يشير لـ ExamTeacher
        public Guid HalqaId { get; set; }         // يشير لـ Halqa

        // Navigation Properties
        public TeacherExam TeacherExams { get; set; } = null!;
        public Halqa Halqa { get; set; } = null!;
        public Guid FoujId { get; set; }
        public DateTime CreatedAt { get; internal set; }
    }
}
