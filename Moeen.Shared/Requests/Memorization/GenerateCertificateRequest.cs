using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Memorization
{
    public class GenerateCertificateRequest
    {
        [Required(ErrorMessage = "Student ID is required")]
        public Guid StudentId { get; set; }

        [Required(ErrorMessage = "Certificate type is required")]
        [StringLength(50)]
        public string CertificateType { get; set; } = "JuzCompletion"; // JuzCompletion / Khatmah

        public int? JuzNumber { get; set; }
    }
}