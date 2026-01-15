using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
    public class Student:User
    {
        public Guid Id { get; set; } 
        public Guid SaturdayHalqeId { get; set; }
        public Guid MosqueId { get; set; }
        public Mosque Mosque { get; set; }
        public int age { get; set; }
        public string gender { get; set; }
        public DateTime enrollmrnt_date { get; set; }
        public int status { get; set; }
        public int score { get; set; }
        public SaturdayHalqe SaturdayHalqe { get; set; }
        public ICollection<ProgressEntry> progressEntrys { get; set; }
        public ICollection <Exam> Exams { get; set; }
        public ICollection<Attendance> Attendances { get; set; }
        public ICollection<ParentSudent>  parentSudents { get; set; }




    }
}
