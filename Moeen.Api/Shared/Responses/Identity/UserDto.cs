using System;

namespace Moeen.Api.Shared.Responses.Identity
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Gender { get; set; }
        public int FontSize { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public string Theme { get; set; }
        public string ProfileImageUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime JoinedAt { get; set; }
    }
}