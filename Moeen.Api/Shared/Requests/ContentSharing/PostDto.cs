using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class PostDto
    {
        public Guid? Id { get; set; } // إذا كان موجوداً فهذا تحديث

        [Required(ErrorMessage = "Title is required")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 200 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Body is required")]
        [StringLength(5000, ErrorMessage = "Body cannot exceed 5000 characters")]
        public string Body { get; set; }

        [Url(ErrorMessage = "Invalid image URL")]
        public string ImageUrl { get; set; }

        public Guid? MosqueId { get; set; } // مرتبط بمسجد (إذا كان عاماً أو خاصاً بحلقة)
        public Guid? HalqaId { get; set; }  // مرتبط بحلقة معينة
    }
}