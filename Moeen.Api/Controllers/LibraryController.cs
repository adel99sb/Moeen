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
        /// البحث في المكتبة
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchLibraryResponse>> SearchLibrary(SearchLibraryRequest request)
        {
            var result = await _libraryService.SearchLibraryAsync(request);
            return Ok(result);
        }
    }
}