using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaQuery
{
    public class GetHalqaStudentsCountRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }
    }
}