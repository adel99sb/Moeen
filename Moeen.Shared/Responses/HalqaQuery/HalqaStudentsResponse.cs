using System.Collections.Generic;
using Moeen.Shared.Responses.Enrollment; // لاستخدام StudentDto

namespace Moeen.Shared.Responses.HalqaQuery
{
    public class HalqaStudentsResponse
    {
        public List<StudentDto> Students { get; set; } = new List<StudentDto>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}