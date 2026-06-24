using System;

namespace Moeen.Shared.Responses.ExamCommand
{
    public class ExamResultDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string HalqaTeacherName { get; set; } = string.Empty;
        public string ExaminerName { get; set; } = string.Empty;
        public int JuzFrom { get; set; }
        public int JuzTo { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; } = string.Empty;
        public int Mark { get; set; }         
        public string Grade { get; set; } = string.Empty;   
    }
}