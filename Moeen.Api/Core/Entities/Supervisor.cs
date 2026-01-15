using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Supervisor:User
    {
        public Guid Id {  get; set; }
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; }
        public DateTime assigned_at { get; set; }
        
    }
}
