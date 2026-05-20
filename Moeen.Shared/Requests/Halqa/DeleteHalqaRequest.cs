using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Halqa
{
    public class DeleteHalqaRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }
    }
}