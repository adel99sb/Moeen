namespace Moeen.Api.Shared.Responses.Exam_Halqa
{
    public class GetExamTeacherByIdResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public ExamTeacherResponse ExamTeacher { get; set; } = null!;

        public GetExamTeacherByIdResponse()
        {
            Success = true;
            Message = "Exam teacher retrieved successfully";
        }
    }
}