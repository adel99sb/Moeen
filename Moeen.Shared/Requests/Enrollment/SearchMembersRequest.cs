using System;

namespace Moeen.Shared.Requests.Enrollment
{
    public class SearchMembersRequest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string MemberType { get; set; } // Student, Teacher, Parent, Supervisor
        public Guid? MosqueId { get; set; }
        public int? Status { get; set; }
        public DateTime? JoinedFrom { get; set; }
        public DateTime? JoinedTo { get; set; }
        public int? Role { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }
}