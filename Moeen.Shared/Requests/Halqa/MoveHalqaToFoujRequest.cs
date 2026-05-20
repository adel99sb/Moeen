using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Halqa
{
    public class MoveHalqaToFoujRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        [Required(ErrorMessage = "Target fouj ID is required")]
        public Guid TargetFoujId { get; set; }
    }
}