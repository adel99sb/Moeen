using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class SaturdayHalqe
    {
        public Guid Id { get; set; }
        public Guid courseId { get; set; }
        public Guid teacherId { get; set; }
        public string name { get; set; }
        public int age_min { get; set; }
        public int age_max { get; set; }
        public SaturdayCourse saturdayCourse { get; set; }
        public ICollection<SaturdayLesson> saturdayLessons { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
