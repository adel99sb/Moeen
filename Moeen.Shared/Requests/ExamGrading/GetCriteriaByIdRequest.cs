using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ExamGrading
{
    public class GetCriteriaByIdRequest
    {
        [Required(ErrorMessage = "Criteria ID is required")]
        public Guid CriteriaId { get; set; }
    }
}