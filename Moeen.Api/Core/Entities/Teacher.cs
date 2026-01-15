using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Teacher:User
    {
        public Guid Id { get; set; }
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; }
        public string boi { get; set; }
        public string assigned_at { get; set; }
        public ICollection<Halqa> halaqas { get; set; }
        public ICollection<ProgressEntry> ProgressEntrys { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
    }
}
