namespace Moeen.Shared.Requests.Mosuq
{
    using System.ComponentModel.DataAnnotations;

    public class AddMosquReq
    {
        [Required]
        [StringLength(150, MinimumLength = 3)]
        public string name { get; set; }

        [Required]
        [StringLength(250)]
        public string address { get; set; }

        [Required]
        [Phone]
        public string contact_phone { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public double Latitude { get; set; }

        [Required]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public double Longitude { get; set; }
    }
}
