using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.ContentSharing
{
    public class PostDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string ImageUrl { get; set; }
        public Guid? MosqueId { get; set; }
        public Guid? HalqaId { get; set; }
        public string MosqueName { get; set; }
        public string HalqaName { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InteractionsCount { get; set; }
        public List<string> MediaUrls { get; set; }
    }
}