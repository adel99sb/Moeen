using Moeen.Shared.Requests.LessonManagement;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.LessonManagement;
using System.Collections.Generic;
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
        /// إدارة وقت الدرس (قديم/متوافق)
        /// </summary>
        Task<ManageLessonTimeResponse> ManageLessonTimeAsync(ManageLessonTimeRequest request);

        /// <summary>
        /// نسخ الدروس بين الحلقات
        /// </summary>
        Task<CopyLessonsResponse> CopyLessonsAsync(CopyLessonsRequest request);

        /// <summary>
        /// [GET] جلب تفاصيل درس محدد بمعرفه مع جميع محتوياته ومواده
        /// </summary>
        Task<LessonDto> GetLessonByIdAsync(GetLessonByIdRequest request);

        /// <summary>
        /// [GET] جلب جميع الدروس التابعة لحلقة معينة مع دعم التصفح والتصفية
        /// </summary>
        Task<PagedList<LessonDto>> GetLessonsByCircleAsync(GetLessonsByCircleRequest request);

        /// <summary>
        /// [GET] جلب المواد والملفات المساعدة المرتبطة بدرس معين
        /// </summary>
        Task<List<LessonMaterialDto>> GetLessonMaterialsAsync(GetLessonMaterialsRequest request);

        /// <summary>
        /// [GET] جلب جدول مواعيد الدروس لحلقة معينة
        /// </summary>
        Task<LessonScheduleDto> GetLessonScheduleAsync(GetLessonScheduleRequest request);

        /// <summary>
        /// [PUT] تحديث جدول مواعيد الدرس
        /// </summary>
        Task<LessonScheduleDto> UpdateLessonScheduleAsync(UpdateLessonScheduleRequest request);

        /// <summary>
        /// [PUT] تعيين درس لحلقات محددة
        /// </summary>
        Task<OperationResponseDto> AssignLessonToCirclesAsync(AssignLessonToCirclesRequest request);

        /// <summary>
        /// [PUT] تعيين معلم مسؤول عن درس معين
        /// </summary>
        Task<OperationResponseDto> AssignTeacherToLessonAsync(AssignTeacherToLessonRequest request);

        /// <summary>
        /// [DELETE] حذف ناعم لدرس
        /// </summary>
        Task<OperationResponseDto> SoftDeleteLessonAsync(SoftDeleteLessonRequest request);

        /// <summary>
        /// [DELETE] إلغاء تعيين درس من حلقة معينة
        /// </summary>
        Task<OperationResponseDto> UnassignLessonFromCircleAsync(UnassignLessonFromCircleRequest request);

        /// <summary>
        /// [POST] تكرار درس موجود لإنشاء درس جديد
        /// </summary>
        Task<LessonDto> DuplicateLessonAsync(DuplicateLessonRequest request);

        /// <summary>
        /// [POST] تسجيل حضور الطلاب في درس معين
        /// </summary>
        Task<LessonAttendanceDto> RecordLessonAttendanceAsync(RecordAttendanceRequest request);
    }
}