using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.ContentSharing
{
    public class ManageAnnouncementRequest
    {
        [Required(ErrorMessage = "Announcement data is required")]
        public AnnouncementDto AnnouncementData { get; set; }
    }
}