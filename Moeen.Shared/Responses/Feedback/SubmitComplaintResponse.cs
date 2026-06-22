namespace Moeen.Shared.Responses.Feedback
{
    public class SubmitComplaintResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? ComplaintId { get; set; }
    }
}