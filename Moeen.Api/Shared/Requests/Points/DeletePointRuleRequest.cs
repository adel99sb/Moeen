using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Points
{
    public class DeletePointRuleRequest
    {
        [Required(ErrorMessage = "Rule ID is required")]
        public Guid RuleId { get; set; }
    }
}