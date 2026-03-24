using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamGrading
{
    public class SetGradingCriteriaRequest
    {
        // إذا كان Id موجوداً فهذا تحديث، وإذا كان null أو Guid.Empty فهذا إنشاء جديد
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        public string Name { get; set; }

        public ExamType? ApplicableTo { get; set; }

        [Required(ErrorMessage = "Min score is required")]
        [Range(0, 100, ErrorMessage = "Min score must be between 0 and 100")]
        public int MinScore { get; set; }

        [Required(ErrorMessage = "Max score is required")]
        [Range(0, 100, ErrorMessage = "Max score must be between 0 and 100")]
        public int MaxScore { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }

        [Required(ErrorMessage = "Points are required")]
        [Range(0, 1000, ErrorMessage = "Points must be between 0 and 1000")]
        public int Points { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string Description { get; set; }

        public bool IsActive { get; set; } = true;
    }
}