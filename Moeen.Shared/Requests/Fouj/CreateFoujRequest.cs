using System;
using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.Fouj
{
    public class CreateFoujRequest
    {
        [Required(ErrorMessage = "«”„ «·›ÊÃ „ÿ·Ê»")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "«”„ «·›ÊÃ ÌÃ» √‰ ÌﬂÊ‰ »Ì‰ 2 Ê 100 Õ—›")]
        public string Name { get; set; }

        [Required(ErrorMessage = "„⁄—› «·„”Ãœ „ÿ·Ê»")]
        public Guid MosqueId { get; set; }

        [Required(ErrorMessage = "Êﬁ  «·»œ«Ì… „ÿ·Ê»")]
        public DateTime StartTime { get; set; }

        [Required(ErrorMessage = "Êﬁ  «·‰Â«Ì… „ÿ·Ê»")]
        public TimeSpan EndTime { get; set; }
    }
}