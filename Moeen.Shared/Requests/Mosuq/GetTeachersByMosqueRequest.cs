using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Mosuq
{
    public class GetTeachersByMosqueRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }

        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;

        [Range(1, 200)]
        public int PageSize { get; set; } = 20;
    }
}