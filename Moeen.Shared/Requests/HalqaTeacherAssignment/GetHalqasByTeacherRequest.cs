using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaTeacherAssignment
{
    public class GetHalqasByTeacherRequest
    {
        [Required(ErrorMessage = "Teacher ID is required")]
        public Guid TeacherId { get; set; }

        /// <summary>
        /// ÅĞÇ true íÑÌÚ ÇáÓÌá ÇáÊÇÑíÎí ÃíÖğÇ
        /// </summary>
        public bool IncludeHistory { get; set; } = false;
    }
}