using Moeen.Api.Shared.Requests.LessonManagement;
using Moeen.Api.Shared.Responses.LessonManagement;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface ILessonManagementService
    {
        /// <summary>
        /// إنشاء درس جديد
        /// </summary>
        Task<LessonDto> CreateLessonAsync(CreateLessonRequest request);

        /// <summary>
        /// تحديث محتوى الدرس
        /// </summary>
        Task<LessonDto> UpdateLessonAsync(UpdateLessonRequest request);

        /// <summary>
        /// حذف درس
        /// </summary>
        Task<DeleteLessonResponse> DeleteLessonAsync(DeleteLessonRequest request);

        /// <summary>
        /// تنظيم تسلسل الدروس
        /// </summary>
        Task<ReorderLessonsResponse> ReorderLessonsAsync(ReorderLessonsRequest request);

        /// <summary>
        /// إضافة مواد مساعدة (ملفات) لدرس
        /// </summary>
        Task<AddLessonMaterialsResponse> AddLessonMaterialsAsync(AddLessonMaterialsRequest request);

        /// <summary>
        /// إدارة وقت الدرس
        /// </summary>
        Task<ManageLessonTimeResponse> ManageLessonTimeAsync(ManageLessonTimeRequest request);

        /// <summary>
        /// نسخ الدروس بين الحلقات
        /// </summary>
        Task<CopyLessonsResponse> CopyLessonsAsync(CopyLessonsRequest request);
    }
}