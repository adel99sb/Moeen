using System.Collections.Generic;

namespace Moeen.Shared.Requests.Enrollment
{
    public class ExportMembersRequest
    {
        public string Format { get; set; } = "Excel"; // Excel, PDF, CSV
        public List<string> Fields { get; set; } = new List<string>(); // List of field names to export
        public string MemberType { get; set; } = string.Empty; // Optional filter
        public Guid? MosqueId { get; set; }
        public string SortBy { get; set; } = "Name";
        public bool SortDescending { get; set; } = false;
    }
}