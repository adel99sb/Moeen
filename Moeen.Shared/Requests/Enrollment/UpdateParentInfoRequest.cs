using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Enrollment
{
    public class UpdateParentInfoRequest
    {
        [Required(ErrorMessage = "Parent ID is required")]
        public Guid ParentId { get; set; }

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Relationship { get; set; }
        public List<Guid> StudentIds { get; set; } = new();
    }
}