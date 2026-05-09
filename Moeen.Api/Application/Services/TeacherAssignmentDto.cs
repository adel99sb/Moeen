namespace Moeen.Api.Application.Services
{
    internal class TeacherAssignmentDto
    {
        public object TeacherId { get; set; }
        public object TeacherName { get; set; }
        public Guid HalqaId { get; set; }
        public string HalqaName { get; set; }
    }
}