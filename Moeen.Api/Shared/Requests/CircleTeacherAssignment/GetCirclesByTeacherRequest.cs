using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.CircleTeacherAssignment
{
    public class GetCirclesByTeacherRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        /// <summary>
        /// ÅĞÇ true íÑÌÚ ÇáÓÌá ÇáÊÇÑíÎí ÃíÖğÇ
        /// </summary>
        public bool IncludeHistory { get; set; } = false;
    }
}