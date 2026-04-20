namespace Moeen.Api.Shared.Requests.Library
{
    public class GetAllBooksRequest
    {
        public string? Category { get; set; }
        public string? Author { get; set; }
        public string? Language { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}