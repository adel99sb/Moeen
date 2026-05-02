namespace Moeen.Shared.Responses.Feedback
{
    public class SubmitComplaintResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? ComplaintId { get; set; }
    }
}