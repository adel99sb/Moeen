namespace Moeen.Shared.Responses.Memorization
{
    public class BatchItemResult
    {
        public int PageNumber { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public int PointsEarned { get; set; }
    }
}