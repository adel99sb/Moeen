using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Attendance
    {
        public Guid Id {  get; set; }
        public Guid saturdayLessonId { get; set; }
        public Guid teacherId { get; set; }
        public Guid studentId { get; set; }
        public Guid halqsessionId { get; set; }
        public SaturdayLesson saturdayLesson { get; set; }
        public Teacher teacher { get; set; }
        public HalqeSession halqeSession { get; set; }
        public Student student { get; set; }
    }
}
