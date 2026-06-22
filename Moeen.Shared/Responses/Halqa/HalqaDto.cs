using System;

namespace Moeen.Shared.Responses.Halqa
{
    public class HalqaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid FoujId { get; set; }
        public string FoujName { get; set; } = string.Empty; // إضافي لعرض اسم الفوج
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty; // إضافي لعرض اسم المعلم
        public string Type { get; set; } = string.Empty;
        public int StudentsCount { get; set; } // يمكن إضافته لاحقاً
    }
}