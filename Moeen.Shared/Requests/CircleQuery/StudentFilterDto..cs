using System;

namespace Moeen.Shared.Requests.CircleQuery
{
    public class StudentFilterDto
    {
        public string Name { get; set; }
        public int? AgeFrom { get; set; }
        public int? AgeTo { get; set; }
        public string Gender { get; set; }
        public int? Status { get; set; }
        public int? MinScore { get; set; }
        public int? MaxScore { get; set; }
        public DateTime? EnrolledFrom { get; set; }
        public DateTime? EnrolledTo { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}