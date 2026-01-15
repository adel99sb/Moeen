using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Entities
{
     public class Mosque
    {
        public Guid Id  { get; set; }
        public string name{ get; set; }
         public string address { get; set; }
        public string contact_phone { get; set; }
        public string Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public ICollection <Fouj> foujs { get; set; }
        public ICollection<Teacher> Teachers { get; set; }
        public ICollection<PdfFile> pdfFiles { get; set; }
        public ICollection<SaturdayCourse> SaturdayCourses { get; set; }
        public ICollection<Post> posts { get; set; }
        public ICollection<Supervisor> supervisors { get; set; }
        public ICollection<Student> Students { get; set; }
    }
}
