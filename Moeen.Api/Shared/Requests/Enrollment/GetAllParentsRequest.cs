namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class GetAllParentsRequest
    {
        public string? Name { get; set; }
        public string? Phone { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}