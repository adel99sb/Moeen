using System;

namespace Moeen.Api.Shared.Responses.LessonManagement
{
    public class LessonMaterialDto
    {
        public Guid Id { get; set; }
        public Guid LessonId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}