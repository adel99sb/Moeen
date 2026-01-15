using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class PdfFile
    {
        public Guid Id {  get; set; }
        public Guid mosqueId { get; set; }
        public Guid SaturdayCourseId { get; set; }
        public string Fileurl { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string uploaded_by { get; set; }
        public DateTime created_at { get; set; }
        public Mosque mosque { get; set; }
        public SaturdayCourse SaturdayCourse { get; set; }
    }
}
