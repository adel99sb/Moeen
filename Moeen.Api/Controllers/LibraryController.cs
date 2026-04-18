using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Library;
using Moeen.Api.Shared.Responses.Library;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LibraryController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public LibraryController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        /// <summary>
        /// إضافة كتاب جديد
        /// </summary>
        [HttpPost("add")]
        public async Task<ActionResult<AddBookResponse>> AddBook(AddBookRequest request)
        {
            var result = await _libraryService.AddBookAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تحديث بيانات كتاب
        /// </summary>
        [HttpPut("update")]
        public async Task<ActionResult<UpdateBookResponse>> UpdateBook(UpdateBookRequest request)
        {
            var result = await _libraryService.UpdateBookInfoAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم/حالي.
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchLibraryResponse>> SearchLibrary(SearchLibraryRequest request)
        {
            var result = await _libraryService.SearchLibraryAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// GET (جديد): بحث سريع عبر Query String.
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<SearchLibraryResponse>> SearchLibraryGet([FromQuery] string query)
        {
            var request = new SearchLibraryRequest { Query = query };
            var result = await _libraryService.SearchLibraryAsync(request);
            return Ok(result);
        }
    }
}