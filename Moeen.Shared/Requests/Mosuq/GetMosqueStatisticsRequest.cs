using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Mosuq
{
    public class GetMosqueStatisticsRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }
    }
}