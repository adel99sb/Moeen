using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ImportExport
{
    public class GetJobStatusRequest
    {
        [Required(ErrorMessage = "Job ID is required")]
        public Guid JobId { get; set; }
    }
}