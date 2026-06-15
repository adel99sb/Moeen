using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.ContentSharing;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IContentSharingService
    {
        /// <summary>
        /// نشر منشور عام أو للحلقات
        /// </summary>
        Task<GeneralResponse> PublishPostAsync(PublishPostRequest request);
        Task<GeneralResponse> GetAvailableHalqasBriefAsync(string? query);

        /// <summary>
        /// التفاعل مع منشور (إعجاب، دعم، إلخ)
        /// </summary>
        Task<GeneralResponse> InteractWithPostAsync(InteractWithPostRequest request);

        /// <summary>
        /// البحث في المحتوى
        /// </summary>
        Task<GeneralResponse> SearchContentAsync(SearchContentRequest request);

        /// <summary>
        /// حذف المحتوى القديم
        /// </summary>
        Task<GeneralResponse> DeleteOldContentAsync(DeleteOldContentRequest request);

        /// <summary>
        /// إدارة الإعلانات (إنشاء/تحديث)
        /// </summary>
        Task<GeneralResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request);

        /// <summary>
        /// إضافة وسائط متعددة (صور) لمنشور
        /// </summary>
        Task<GeneralResponse> AddMultimediaAsync(AddMultimediaRequest request);

        /// <summary>
        /// [GET] جلب منشور محدد بمعرفه مع تفاصيل التفاعلات
        /// </summary>
        Task<GeneralResponse> GetPostByIdAsync(GetPostByIdRequest request);

        /// <summary>
        /// [GET] جلب قائمة التفاعلات على منشور معين
        /// </summary>
        Task<GeneralResponse> GetPostInteractionsAsync(GetPostInteractionsRequest request);

        /// <summary>
        /// [PUT] تحديث محتوى منشور موجود
        /// </summary>
        Task<GeneralResponse> UpdatePostAsync(UpdatePostRequest request);

        /// <summary>
        /// [DELETE] حذف منشور نهائيًا
        /// </summary>
        Task<GeneralResponse> DeletePostAsync(DeletePostRequest request);

        /// <summary>
        /// [GET] جلب كافة المنشورات
        /// </summary>
        Task<GeneralResponse> GetAllPostsAsync();
    }
}