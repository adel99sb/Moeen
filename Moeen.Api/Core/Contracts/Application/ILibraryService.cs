using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ILibraryService
    {
        /// <summary>
        /// إضافة كتاب جديد إلى المكتبة
        /// </summary>
        Task<GeneralResponse> AddBookAsync(AddBookRequest request);

        /// <summary>
        /// تحديث بيانات كتاب موجود
        /// </summary>
        Task<GeneralResponse> UpdateBookInfoAsync(UpdateBookRequest request);

        /// <summary>
        /// البحث في فهرس المكتبة
        /// </summary>
        Task<GeneralResponse> SearchLibraryAsync(SearchLibraryRequest request);

        /// <summary>
        /// [GET] جلب جميع الكتب مع التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل كتاب محدد بالمعرف
        /// </summary>
        Task<GeneralResponse> GetBookByIdAsync(GetBookByIdRequest request);

        /// <summary>
        /// [GET] جلب الكتب حسب التصنيف
        /// </summary>
        Task<GeneralResponse> GetBooksByCategoryAsync(GetBooksByCategoryRequest request);

        /// <summary>
        /// [PUT] تحديث تصنيفات كتاب (إضافة/إزالة)
        /// </summary>
        Task<GeneralResponse> UpdateBookCategoriesAsync(UpdateBookCategoriesRequest request);

        /// <summary>
        /// [DELETE] حذف كتاب نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteBookAsync(DeleteBookRequest request);
    }
}