using Moeen.Shared.Constants;
using System;

namespace Moeen.Shared.Responses.ContentSharing
{
    public class InteractionDto
    {
        public Guid Id { get; set; }
        public Guid PostId { get; set; }

        public Guid UserId { get; set; }
        public string UserName { get; set; } = string.Empty;

        public InteractionType Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}