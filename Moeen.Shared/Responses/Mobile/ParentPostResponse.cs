using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class ParentPostResponse
    {
        public List<ParentPostDto> Posts { get; set; } = new();
    }

    public class ParentPostDto
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
        public bool IsLikedByCurrentParent { get; set; }
        public List<string> VisibleForChildren { get; set; } = new();
    }

    public class ParentPostInteractionResponse
    {
        public Guid PostId { get; set; }
        public bool IsLikedByCurrentParent { get; set; }
        public int InteractionsCount { get; set; }
    }
}
