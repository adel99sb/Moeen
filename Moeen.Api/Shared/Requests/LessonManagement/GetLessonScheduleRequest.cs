using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.LessonManagement
{
    public class GetLessonScheduleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }
    }
}