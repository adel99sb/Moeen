namespace Moeen.Api.Shared.Responses.Enrollment
{
    public class MemberStatisticsDto
    {
        public int TotalMembers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalParents { get; set; }
        public int TotalSupervisors { get; set; }

        public int ActiveMembers { get; set; }
        public int SuspendedMembers { get; set; }
        public int GraduatedMembers { get; set; }
    }
}