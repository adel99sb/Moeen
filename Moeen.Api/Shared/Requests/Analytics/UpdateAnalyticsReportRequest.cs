using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.Analytics
{
    public class UpdateAnalyticsReportRequest
    {
        /// <summary>ÚäæÇä ÇáÊŞÑíÑ ÈÚÏ ÇáÊÍÏíË</summary>
        [StringLength(200)]
        public string? Title { get; set; }

        /// <summary>ÈíÇäÇÊ ÇáÊŞÑíÑ ÇáÌÏíÏÉ ÈÕíÛÉ JSON</summary>
        [Required]
        public string PayloadJson { get; set; } = string.Empty;
    }
}