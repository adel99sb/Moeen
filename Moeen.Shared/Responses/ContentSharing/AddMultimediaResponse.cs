namespace Moeen.Shared.Responses.ContentSharing
{
    public class AddMultimediaResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int AddedCount { get; set; }
    }
}