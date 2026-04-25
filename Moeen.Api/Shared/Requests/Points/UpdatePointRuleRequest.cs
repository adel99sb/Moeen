using Moeen.Api.Core.Constants;
using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class UpdatePointRuleRequest
    {
        public Guid? RuleId { get; set; }

        [Required(ErrorMessage = "Grade is required")]
        public Grade Grade { get; set; }

        [Required(ErrorMessage = "Points is required")]
        [Range(0, 1000)]
        public int Points { get; set; }

        public bool IsActive { get; set; } = true;
    }
}