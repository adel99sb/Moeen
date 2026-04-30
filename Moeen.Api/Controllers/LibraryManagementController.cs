using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Library;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/library-management")]
    [ApiController]
    public class LibraryManagementController : ControllerBase
    {
        private readonly ILibraryService _libraryService;

        public LibraryManagementController(ILibraryService libraryService)
        {
            _libraryService = libraryService;
        }

        /// <summary>
        /// GET: Ã·» Ã„Ì⁄ «·ﬂ » „⁄ «·›· —… Ê«· —ﬁÌ„.
        /// </summary>
        [HttpGet("books")]
        public async Task<ActionResult<PagedList<BookResponseDto>>> GetAllBooks([FromQuery] GetAllBooksRequest request)
            => Ok(await _libraryService.GetAllBooksAsync(request));

        /// <summary>
        /// GET: Ã·» ﬂ «» »«·„⁄—›.
        /// </summary>
        [HttpGet("books/{bookId:guid}")]
        public async Task<ActionResult<BookResponseDto>> GetBookById([FromRoute] Guid bookId)
            => Ok(await _libraryService.GetBookByIdAsync(new GetBookByIdRequest { BookId = bookId }));

        /// <summary>
        /// GET: Ã·» «·ﬂ » Õ”» «· ’‰Ì›.
        /// </summary>
        [HttpGet("books/by-category")]
        public async Task<ActionResult<List<BookResponseDto>>> GetBooksByCategory([FromQuery] string category)
            => Ok(await _libraryService.GetBooksByCategoryAsync(new GetBooksByCategoryRequest { Category = category }));

        /// <summary>
        /// POST: ≈÷«›… ﬂ «».
        /// </summary>
        [HttpPost("books")]
        public async Task<ActionResult<AddBookResponse>> AddBook([FromBody] AddBookRequest request)
            => Ok(await _libraryService.AddBookAsync(request));

        /// <summary>
        /// PUT:  ÕœÌÀ »Ì«‰«  ﬂ «».
        /// </summary>
        [HttpPut("books")]
        public async Task<ActionResult<UpdateBookResponse>> UpdateBook([FromBody] UpdateBookRequest request)
            => Ok(await _libraryService.UpdateBookInfoAsync(request));

        /// <summary>
        /// PUT:  ÕœÌÀ  ’‰Ì›«  ﬂ «».
        /// </summary>
        [HttpPut("books/{bookId:guid}/categories")]
        public async Task<ActionResult<OperationResponseDto>> UpdateBookCategories(
            [FromRoute] Guid bookId,
            [FromBody] UpdateBookCategoriesRequest request)
        {
            request.BookId = bookId;
            return Ok(await _libraryService.UpdateBookCategoriesAsync(request));
        }

        /// <summary>
        /// DELETE: Õ–› ﬂ «» ‰Â«∆Ì«.
        /// </summary>
        [HttpDelete("books/{bookId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteBook([FromRoute] Guid bookId)
            => Ok(await _libraryService.DeleteBookAsync(new DeleteBookRequest { BookId = bookId }));

        /// <summary>
        /// POST („ Ê«›ﬁ): «·»ÕÀ ›Ì «·„ﬂ »….
        /// </summary>
        [HttpPost("search")]
        public async Task<ActionResult<SearchLibraryResponse>> SearchLibrary([FromBody] SearchLibraryRequest request)
            => Ok(await _libraryService.SearchLibraryAsync(request));
    }
}