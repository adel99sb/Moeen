using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Analytics
{
    public class SaveAnalyticsReportRequest
    {
        /// <summary>‰Ê⁄ «· ﬁ—Ì—: Student / Teacher / Circle</summary>
        [Required]
        [StringLength(50)]
        public string ReportType { get; set; } = string.Empty;

        /// <summary>⁄‰Ê«‰ «Œ Ì«—Ì ·· ﬁ—Ì—</summary>
        [StringLength(200)]
        public string? Title { get; set; }

        /// <summary>»Ì«‰«  «· ﬁ—Ì— »’Ì€… JSON</summary>
        [Required]
        public string PayloadJson { get; set; } = string.Empty;

        public Guid? StudentId { get; set; }
        public Guid? TeacherId { get; set; }
        public Guid? CircleId { get; set; }
    }
}