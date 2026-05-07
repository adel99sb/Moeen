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
        public string title { get; set; }
        public string description { get; set; }
        public Mosque Mosque { get; set; } 
        public ICollection<PdfFile> pdfFiles { get; set; }
        public ICollection<SaturdayHalqa> saturdayHalqes { get; set; }


    }
}
