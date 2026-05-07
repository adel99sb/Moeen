using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class DeleteOldContentRequest
    {
        [Required(ErrorMessage = "Older than date is required")]
        public DateTime OlderThan { get; set; }
    }
}
