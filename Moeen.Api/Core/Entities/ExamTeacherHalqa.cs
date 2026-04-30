namespace Moeen.Api.Core.Entities
{
    public class ExamTeacherHalqa
    {
        public Guid Id { get; set; }

        // Foreign Keys
        public Guid ExamTeacherId { get; set; }  // يشير لـ ExamTeacher
        public Guid HalqaId { get; set; }         // يشير لـ Halqa

        // Navigation Properties
        public TeacherExam TeacherExams { get; set; }
        public Halqa Halqa { get; set; }
        public Fouj Fouj { get; set; }

    }
}
