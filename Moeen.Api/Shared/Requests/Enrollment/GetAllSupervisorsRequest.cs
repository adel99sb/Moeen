namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class GetAllSupervisorsRequest
    {
        public string? Name { get; set; }
        public int? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}