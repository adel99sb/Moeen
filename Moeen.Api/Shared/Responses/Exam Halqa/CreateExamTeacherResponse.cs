namespace Moeen.Api.Shared.Responses.Exam_Halqa
{
    public class CreateExamTeacherResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public ExamTeacherResponse ExamTeacher { get; set; }

        public CreateExamTeacherResponse()
        {
            Success = true;
            Message = "Exam teacher created successfully";
        }
    }
}
