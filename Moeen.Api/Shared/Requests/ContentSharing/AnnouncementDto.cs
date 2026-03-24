using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class AnnouncementDto
    {
        public Guid? Id { get; set; }

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Content is required")]
        [StringLength(5000, ErrorMessage = "Content cannot exceed 5000 characters")]
        public string Content { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid? MosqueId { get; set; }
        public Guid? HalqaId { get; set; }
    }
}