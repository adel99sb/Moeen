using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.HalqaTeacherAssignment
{
    public class ReplaceTeacherRequest
    {
        [Required(ErrorMessage = "Halqa ID is required")]
        public Guid HalqaId { get; set; }

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