using Microsoft.AspNetCore.Identity;
using Moeen.Shared.Constants;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Core.Entities
{
    public class User : IdentityUser<Guid>
    {
        [StringLength(50)]
        public string? PhoneNumber { get; set; }
        [StringLength(50)]
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public string? ProfileImageUrl { get; set; }

        [Required]
        public UserRole UserRole { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // جداول الكسر والعلاقات المتبادلة
        public ICollection<MosqueUser> MosqueUsers { get; set; } = new List<MosqueUser>();
        public ICollection<ParentStudent> AsParentStudents { get; set; } = new List<ParentStudent>();
        public ICollection<ParentStudent> AsChildStudents { get; set; } = new List<ParentStudent>();
    }
}