using System;

namespace Moeen.Shared.Requests.Enrollment
{
    public class GetAllStudentsRequest
    {
        public string? Name { get; set; }
        public Guid? MosqueId { get; set; }
        public int? Status { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}