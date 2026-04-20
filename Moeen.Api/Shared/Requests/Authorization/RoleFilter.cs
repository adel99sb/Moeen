namespace Moeen.Api.Shared.Requests.Authorization
{
    public class RoleFilter
    {
        public string? Name { get; set; }
        public string? Permission { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}