using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Post
    {
        public Guid Id {  get; set; }
        public Guid mosqueId { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string imageurl { get; set; }
        public DateTime created_at { get; set; }
        public Mosque mosque { get; set; }
        public ICollection<PosInteraction> postinteractions { get; set; }


    }
}
