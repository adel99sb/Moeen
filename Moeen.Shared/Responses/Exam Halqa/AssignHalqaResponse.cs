namespace Moeen.Api.Shared.Responses.Exam_Halqa
{
    public class AssignHalqaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        public AssignHalqaResponse()
        {
            Success = true;
            Message = "Halqa assigned successfully";
        }
    }
}
