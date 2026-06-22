using Moeen.Api.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Enities
{
    public class ParentSudent : User
    {
        public new Guid Id { get; set; }
        public Guid userid { get; set; }
        public Guid Studentid { get; set; }
        public Student Student { get; set; } = null!;

    }
}
