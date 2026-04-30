using System;

namespace Moeen.Shared.Responses.Circle
{
    public class CircleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid FoujId { get; set; }
        public string FoujName { get; set; } // إضافي لعرض اسم الفوج
        public Guid TeacherId { get; set; }
        public string TeacherName { get; set; } // إضافي لعرض اسم المعلم
        public string Type { get; set; }
        public int StudentsCount { get; set; } // يمكن إضافته لاحقاً
    }
}