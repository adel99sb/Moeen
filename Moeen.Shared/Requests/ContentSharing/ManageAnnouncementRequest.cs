using System.ComponentModel.DataAnnotations;

namespace Moeen.Shared.Requests.ContentSharing
{
    public class ManageAnnouncementRequest
    {
        [Required(ErrorMessage = "Announcement data is required")]
        public AnnouncementDto AnnouncementData { get; set; } = null!;
    }
}