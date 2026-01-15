using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class PosInteraction 
    {
        public Guid Id {  get; set; }
        public Guid postId { get; set; }
        public DateTime date { get; set; }
        public Post post { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }

    }
}
