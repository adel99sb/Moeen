using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Halqa
    {
        public Guid Id { get; set; }
        public Guid foujId { get; set; }
        public string Name { get; set; }
        public Guid teacherId { get; set; }
        public string type { get; set; }
        public Teacher Teacher { get; set; }
        public Fouj Fouj { get; set; }
        public ICollection<ProgressEntry> ProgressEntrys { get; set; }
        public ICollection<HalqeSession> HalqeSession { get; set; }


    }
}
