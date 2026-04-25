using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Mosuq
{
    public class GetCirclesByMosqueRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }
    }
}