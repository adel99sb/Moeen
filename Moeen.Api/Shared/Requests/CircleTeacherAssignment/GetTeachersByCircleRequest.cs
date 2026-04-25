using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.CircleTeacherAssignment
{
    public class GetTeachersByCircleRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        /// <summary>
        /// ÅĞÇ true íÑÌÚ ÇáãÚáãíä ÛíÑ ÇáäÔØíä ÃíÖğÇ
        /// </summary>
        public bool IncludeInactive { get; set; } = false;
    }
}