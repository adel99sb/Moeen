using System;

namespace Moeen.Api.Shared.Responses.Library
{
    public class BookResponseDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string ISBN { get; set; }
        public string Publisher { get; set; }
        public int? PublicationYear { get; set; }
        public string Description { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}