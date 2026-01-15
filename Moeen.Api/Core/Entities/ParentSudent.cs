using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class ParentSudent:User
    {
        public Guid Id { get; set; } 
        public Guid userId { get; set; }
        public Guid StudentId { get; set; }
        public Student Student {  get; set; }

    }
}
