using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [ApiController]
    [Authorize(Roles = nameof(Roles.ParentSudent))]
    [Route("api/mobile/parent-library")]
    public class MobileParentLibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public MobileParentLibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        [HttpGet("books")]
        public async Task<ActionResult<GeneralResponse>> GetBooks(
            [FromQuery] string? query,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20)
        {
            var safePageNumber = pageNumber > 0 ? pageNumber : 1;
            var safePageSize = pageSize is > 0 and <= 100 ? pageSize : 20;

            var response = await _libraryService.GetAllBooksAsync(new GetAllBooksRequest
            {
                PageNumber = safePageNumber,
                PageSize = safePageSize,
                Category = string.IsNullOrWhiteSpace(query) ? null : query.Trim()
            });

            return StatusCode(response.StatusCode, response);
        }
    }
}
