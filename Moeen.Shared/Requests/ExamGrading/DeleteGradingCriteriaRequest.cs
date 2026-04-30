using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamGrading
{
    public class DeleteGradingCriteriaRequest
    {
        [Required(ErrorMessage = "Criteria ID is required")]
        public Guid CriteriaId { get; set; }
    }
}