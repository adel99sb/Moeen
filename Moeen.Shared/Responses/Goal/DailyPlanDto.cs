using Moeen.Shared.Requests.Goal;
using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Goal
{
    public class DailyPlanDto
    {
        public Guid StudentId { get; set; }
        public DateTime Date { get; set; }
        public DailyGoalDto Goal { get; set; } = null!;
        public List<PlanItemDto> Items { get; set; } = new List<PlanItemDto>(); // تفاصيل المهام
        public int ProgressPercentage { get; set; } // نسبة الإنجاز
    }

    public class PlanItemDto
    {
        public string Type { get; set; } = string.Empty; // "NewPage", "ReviewJuz", "ReviewPage"
        public int Target { get; set; }
        public int Completed { get; set; }
        public bool IsCompleted => Completed >= Target;
    }
}