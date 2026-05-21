using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Fouj
{
    public class UpdateFoujRequest
    {
        [Required(ErrorMessage = "„⁄—› «·›ÊÃ „ÿ·Ê»")]
        public Guid FoujId { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "«”„ «·›ÊÃ ÌÃ» √‰ ÌﬂÊ‰ »Ì‰ 2 Ê 100 Õ—›")]
        public string Name { get; set; }

        public Guid? MosqueId { get; set; }
        public DateTime? StartTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}