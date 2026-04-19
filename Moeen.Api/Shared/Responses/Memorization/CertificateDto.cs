using System;

namespace Moeen.Api.Shared.Responses.Memorization
{
    public class CertificateDto
    {
        public Guid CertificateId { get; set; }
        public Guid StudentId { get; set; }
        public string CertificateType { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
    }
}