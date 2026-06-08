using System;
using Moeen.Shared.Constants;

namespace Moeen.Shared.Responses.DailyAssignments
{
    public class DailyAssignmentDto
    {
        public Guid AssignmentId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime Date { get; set; }

        public ProgressRecordType AssignmentType { get; set; }
        public ReviewType Status { get; set; }

        public int JuzNumber { get; set; }
        public int FromPage { get; set; }
        public int ToPage { get; set; }

        public Grade? Grade { get; set; }
        public int Points { get; set; }
        public string? Notes { get; set; }
    }
}