using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Fouj
    {
        public  Guid Id {  get; set; }
        public string name { get; set; } 
        public DateTime start_time { get; set; }
        public TimeSpan End_time { get; set; }
        public Guid mosqueId { get; set; } 
        public Mosque Mosque { get; set; } 
        public ICollection<Halqa> halqas { get; set; }

        

    }
}
