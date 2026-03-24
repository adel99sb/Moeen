using Moeen.Api.Shared.Requests.Library;
using Moeen.Api.Shared.Responses.Library;
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
    }
}