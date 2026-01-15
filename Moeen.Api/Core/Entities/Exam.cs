using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Exam
    {
        public Guid Id {  get; set; }
        public Guid studentId { get; set; }
        public Guid teacherId { get; set; }
        public int juz_form {  get; set; }
        public int juz_to { get; set; }
        public int score { get; set; }
        public DateTime date { get; set; }
        public string notes { get; set; }
        public int mark { get; set; }
        public Student Student { get; set; }


    }
}
