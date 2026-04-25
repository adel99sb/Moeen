using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Enrollment
{
    public class DeleteParentRequest
    {
        [Required(ErrorMessage = "Parent ID is required")]
        public Guid ParentId { get; set; }
    }
}