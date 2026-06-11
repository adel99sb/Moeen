using System.ComponentModel.DataAnnotations.Schema;

namespace Moeen.Api.Core.Entities
{
    public class ParentStudent
    {
        public Guid ParentId { get; set; }

        [ForeignKey("ParentId")]
        public User Parent { get; set; }

        public Guid StudentId { get; set; }

        [ForeignKey("StudentId")]
        public User Student { get; set; }
    }
}
