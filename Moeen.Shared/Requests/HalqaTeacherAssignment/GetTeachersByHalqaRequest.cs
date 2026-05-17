using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.CircleTeacherAssignment
{
    public class GetTeachersByHalqaRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

        /// <summary>
        /// ÅĞÇ true íÑÌÚ ÇáãÚáãíä ÛíÑ ÇáäÔØíä ÃíÖğÇ
        /// </summary>
        public bool IncludeInactive { get; set; } = false;
    }
}