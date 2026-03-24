using System;

namespace Moeen.Api.Shared.Responses.ExamQuery
{
    public class ExamResultDto
    {
        public Guid Id { get; set; }
        public Guid StudentId { get; set; }
        public string StudentName { get; set; }
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; }
        public int JuzFrom { get; set; }
        public int JuzTo { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
        public string Notes { get; set; }
        public int Mark { get; set; }
        public string Grade { get; set; }
    }
}