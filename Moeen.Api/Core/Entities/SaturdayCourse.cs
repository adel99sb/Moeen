using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class SaturdayCourse
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public string title { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public Mosque Mosque { get; set; } = null!; 
        public ICollection<PdfFile> pdfFiles { get; set; } = new List<PdfFile>();
        public ICollection<SaturdayHalqa> saturdayHalqes { get; set; } = new List<SaturdayHalqa>();


    }
}
