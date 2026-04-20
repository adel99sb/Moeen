using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ExamGrading
{
    public class GetCriteriaByIdRequest
    {
        [Required(ErrorMessage = "Criteria ID is required")]
        public Guid CriteriaId { get; set; }
    }
}