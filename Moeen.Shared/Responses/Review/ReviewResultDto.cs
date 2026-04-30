namespace Moeen.Shared.Responses.Review
{
    public class ReviewResultDto
    {
        public int JuzNumber { get; set; }
        public bool Success { get; set; }
        public int PointsEarned { get; set; }
        public string Notes { get; set; }
        public string ErrorMessage { get; set; }  // في حال فشل التسجيل
    }
}