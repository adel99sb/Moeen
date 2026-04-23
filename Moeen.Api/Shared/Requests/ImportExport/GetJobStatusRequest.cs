using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ImportExport
{
    public class GetJobStatusRequest
    {
        [Required(ErrorMessage = "Job ID is required")]
        public Guid JobId { get; set; }
    }
}