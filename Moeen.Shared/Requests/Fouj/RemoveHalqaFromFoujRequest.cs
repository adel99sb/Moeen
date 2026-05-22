using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Fouj
{
    public class RemoveHalqaFromFoujRequest
    {
        [Required(ErrorMessage = "ãÚÑİ ÇáİæÌ ãØáæÈ")]
        public Guid FoujId { get; set; }

        [Required(ErrorMessage = "ãÚÑİ ÇáÍáŞÉ ãØáæÈ")]
        public Guid HalqaId { get; set; }
    }
}