namespace Moeen.Shared.Responses.LessonManagement
{
    public class CopyLessonsResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public int LessonsCopiedCount { get; set; }
    }
}