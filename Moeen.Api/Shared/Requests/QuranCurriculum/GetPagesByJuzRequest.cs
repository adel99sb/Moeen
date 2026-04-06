using System.ComponentModel.DataAnnotations;

namespace Moeen.Api.Shared.Requests.QuranCurriculum
{
    public class GetPagesByJuzRequest
    {
        [Required(ErrorMessage = "Juz number is required")]
        [Range(1, 30, ErrorMessage = "Juz number must be between 1 and 30")]
        public int JuzNumber { get; set; }
    }
}