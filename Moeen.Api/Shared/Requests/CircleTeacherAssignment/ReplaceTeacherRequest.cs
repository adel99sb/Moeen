using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.CircleTeacherAssignment
{
    public class ReplaceTeacherRequest
    {
        [Required(ErrorMessage = "Circle ID is required")]
        public Guid CircleId { get; set; }

        [Required(ErrorMessage = "Old teacher ID is required")]
        public Guid OldTeacherId { get; set; }

        [Required(ErrorMessage = "New teacher ID is required")]
        public Guid NewTeacherId { get; set; }

        /// <summary>
        /// ÅĞÇ true íÈŞì ÇáãÚáã ÇáŞÏíã ßãÓÇÚÏ (ÛíÑ ÃÓÇÓí)
        /// </summary>
        public bool KeepOldAsSecondary { get; set; } = false;
    }
}