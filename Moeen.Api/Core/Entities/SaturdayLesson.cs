using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class SaturdayLesson
    {
        public Guid Id {  get; set; }
        public Guid halqaId { get; set; }
        public int lesson_number { get; set; }
        public TimeSpan start_time { get; set; }
        public TimeSpan end_time { get; set; }
        public SaturdayHalqe SaturdayHalqe { get; set; }
        public ICollection<Attendance> attendances { get; set; }

    }
}
