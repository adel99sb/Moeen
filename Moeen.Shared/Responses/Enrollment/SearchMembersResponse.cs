using System.Collections.Generic;

namespace Moeen.Shared.Responses.Enrollment
{
    public class SearchMembersResponse
    {
        public List<MemberDto> Members { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}