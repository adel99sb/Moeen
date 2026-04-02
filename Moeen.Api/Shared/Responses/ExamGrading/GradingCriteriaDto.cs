using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Enums;
using System;

namespace Moeen.Api.Shared.Responses.ExamGrading
{
    public class GradingCriteriaDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ExamType? ApplicableTo { get; set; } // نوع الاختبار المطبق عليه (null يعني شامل)
        public int MinScore { get; set; }
        public int MaxScore { get; set; }
        public Grade Grade { get; set; } // التقدير (ممتاز، جيد جداً، ...)
        public int Points { get; set; } // النقاط المكتسبة
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

