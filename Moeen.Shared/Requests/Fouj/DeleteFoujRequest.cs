using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Fouj
{
    public class DeleteFoujRequest
    {
        [Required(ErrorMessage = "ãÚÑİ ÇáİæÌ ãØáæÈ")]
        public Guid FoujId { get; set; }
    }
}