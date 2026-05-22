using Moeen.Shared.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamCommand
{
    public class CreateLabExamRequest
    {
        [Required]
        public Guid StudentId { get; set; }
        
        [Required]
        public Guid TeacherId { get; set; }
        
        public DateTime ExamDate { get; set; }
        
        public string? FoujName { get; set; }
        
        public ExamType ExamType { get; set; }

        [Range(0, 100)]
        public int Grade { get; set; } // ”‰ﬁÊ„ »Õ›ŸÂ« ›Ì Õﬁ· Score ›Ì «·ﬂÌ«‰

        public string? Rating { get; set; } // ”‰ﬁÊ„ »œ„ÃÂ« ›Ì Õﬁ· Notes

        public int PointsAwarded { get; set; } // ”‰ﬁÊ„ »Õ›ŸÂ« ›Ì Õﬁ· Mark ›Ì «·ﬂÌ«‰

        public string? Notes { get; set; }
    }
}