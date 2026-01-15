using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Complaint
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string content { get; set; }
        public DateTime created_at { get; set; }
        public User user { get; set; }

    }
}
