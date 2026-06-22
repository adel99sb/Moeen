using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class StudentPostResponse
    {
        public List<StudentPostDto> Posts { get; set; } = new();
    }

    public class StudentPostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public Guid? MosqueId { get; set; }
        public Guid? HalqaId { get; set; }
        public string? MosqueName { get; set; }
        public string? HalqaName { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsAnnouncement { get; set; }
        public int InteractionsCount { get; set; }
        public bool IsLikedByCurrentStudent { get; set; }
    }

    public class StudentPostInteractionResponse
    {
        public Guid PostId { get; set; }
        public bool IsLikedByCurrentStudent { get; set; }
        public int InteractionsCount { get; set; }
    }
}
