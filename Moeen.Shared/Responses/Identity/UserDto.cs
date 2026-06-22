using System;

namespace Moeen.Shared.Responses.Identity
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int FontSize { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Theme { get; set; } = string.Empty;
        public string ProfileImageUrl { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}