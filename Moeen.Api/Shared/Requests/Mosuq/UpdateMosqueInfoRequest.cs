using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Mosuq
{
    public class UpdateMosqueInfoRequest
    {
        [Required(ErrorMessage = "Mosque ID is required")]
        public Guid MosqueId { get; set; }

        [StringLength(150, MinimumLength = 3)]
        public string? Name { get; set; }

        [StringLength(250)]
        public string? Address { get; set; }

        [Phone]
        public string? ContactPhone { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        [Range(1, 10000)]
        public int? Capacity { get; set; }

        [Range(-90, 90)]
        public double? Latitude { get; set; }

        [Range(-180, 180)]
        public double? Longitude { get; set; }
    }
}