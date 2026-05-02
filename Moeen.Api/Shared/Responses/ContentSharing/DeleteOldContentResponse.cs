namespace Moeen.Api.Shared.Responses.ContentSharing
{
    public class DeleteOldContentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int DeletedCount { get; set; }
    }
}