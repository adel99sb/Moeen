using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaQuery
{
    public class GetHalqaStudentsRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        public StudentFilterDto Filter { get; set; } = null!; // يمكن أن يكون null
    }
}