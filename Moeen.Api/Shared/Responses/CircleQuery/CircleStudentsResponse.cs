using System.Collections.Generic;
using Moeen.Api.Shared.Responses.Enrollment; // لاستخدام StudentDto

namespace Moeen.Api.Shared.Responses.CircleQuery
{
    public class CircleStudentsResponse
    {
        public List<StudentDto> Students { get; set; }
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    }
}