using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Library;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ILibraryService
    {
        /// <summary>
        /// إضافة كتاب جديد إلى المكتبة
        /// </summary>
        Task<AddBookResponse> AddBookAsync(AddBookRequest request);

        /// <summary>
        /// تحديث بيانات كتاب موجود
        /// </summary>
        Task<UpdateBookResponse> UpdateBookInfoAsync(UpdateBookRequest request);

        /// <summary>
        /// البحث في فهرس المكتبة
        /// </summary>
        Task<SearchLibraryResponse> SearchLibraryAsync(SearchLibraryRequest request);

        /// <summary>
        /// [GET] جلب جميع الكتب مع التصفح والتصفية
        /// </summary>
        Task<PagedList<BookResponseDto>> GetAllBooksAsync(GetAllBooksRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل كتاب محدد بالمعرف
        /// </summary>
        Task<BookResponseDto> GetBookByIdAsync(GetBookByIdRequest request);

        /// <summary>
        /// [GET] جلب الكتب حسب التصنيف
        /// </summary>
        Task<List<BookResponseDto>> GetBooksByCategoryAsync(GetBooksByCategoryRequest request);

        /// <summary>
        /// [PUT] تحديث تصنيفات كتاب (إضافة/إزالة)
        /// </summary>
        Task<OperationResponseDto> UpdateBookCategoriesAsync(UpdateBookCategoriesRequest request);

        /// <summary>
        /// [DELETE] حذف كتاب نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteBookAsync(DeleteBookRequest request);
    }
}