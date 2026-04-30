using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Mosuq
{
    public class GetTeachersByMosqueRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }
    }
}