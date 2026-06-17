using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IEnrollmentService
    {
        /// <summary>
        /// تسجيل طالب جديد
        /// </summary>
        Task<GeneralResponse> RegisterStudentAsync(RegisterStudentRequest request);

        /// <summary>
        /// إضافة معلم جديد للكادر
        /// </summary>
        Task<GeneralResponse> AddTeacherAsync(AddTeacherRequest request);

        /// <summary>
        /// [POST] إضافة مشرف جديد بواسطة المالك
        /// </summary>
        Task<GeneralResponse> AddSupervisorAsync(AddSupervisorRequest request);

        /// <summary>
        /// [POST] ترقية معلم ليصبح مشرفاً
        /// </summary>
        Task<GeneralResponse> PromoteTeacherToSupervisorAsync(PromoteTeacherToSupervisorRequest request);

        /// <summary>
        /// تسجيل ولي أمر جديد
        /// </summary>
        Task<GeneralResponse> RegisterParentAsync(RegisterParentRequest request);

        /// <summary>
        /// تحديث معلومات عضو (عام)
        /// </summary>
        Task<GeneralResponse> UpdateMemberInfoAsync(UpdateMemberInfoRequest request);

        /// <summary>
        /// البحث عن أعضاء بناءً على معايير محددة
        /// </summary>
        Task<GeneralResponse> SearchMembersAsync(SearchMembersRequest request);


        Task<GeneralResponse> CancelMembershipAsync(CancelMembershipRequest request);

        /// <summary>
        /// الحصول على الملف الشخصي الكامل لعضو معين
        /// </summary>
        Task<GeneralResponse> GetMemberProfileAsync(GetMemberProfileRequest request);

        /// <summary>
        /// تحديث حالة العضو (نشط، موقوف، متخرج)
        /// </summary>
        Task<GeneralResponse> UpdateMemberStatusAsync(UpdateMemberStatusRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع الطلاب مع التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetAllStudentsAsync(GetAllStudentsRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع المعلمين مع التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetAllTeachersAsync(GetAllTeachersRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع أولياء الأمور مع التصفح والتصفية
        /// </summary>
        Task<GeneralResponse> GetAllParentsAsync(GetAllParentsRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع المشرفين (لواجهة المالك)
        /// </summary>
        Task<GeneralResponse> GetAllSupervisorsAsync(GetAllSupervisorsRequest request);

        /// <summary>
        /// [GET] جلب الأبناء المرتبطين بولي أمر معين
        /// </summary>
        Task<GeneralResponse> GetChildrenByParentAsync(GetChildrenByParentRequest request);

        /// <summary>
        /// [GET] إحصائيات الأعضاء (حسب النوع/الحالة/المسجد)
        /// </summary>
        Task<GeneralResponse> GetMemberStatisticsAsync(GetMemberStatisticsRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات طالب محددة
        /// </summary>
        Task<GeneralResponse> UpdateStudentInfoAsync(UpdateStudentInfoRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات معلم محددة
        /// </summary>
        Task<GeneralResponse> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات ولي أمر محددة
        /// </summary>
        Task<GeneralResponse> UpdateParentInfoAsync(UpdateParentInfoRequest request);

        /// <summary>
        /// [DELETE] حذف سجل طالب نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteStudentAsync(DeleteStudentRequest request);

        /// <summary>
        /// [DELETE] حذف سجل معلم نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteTeacherAsync(DeleteTeacherRequest request);

        /// <summary>
        /// [DELETE] حذف سجل ولي أمر نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteParentAsync(DeleteParentRequest request);

        /// <summary>
        /// [DELETE] حذف سجل مشرف نهائيًا
        /// </summary>
        Task<GeneralResponse> DeleteSupervisorAsync(DeleteSupervisorRequest request);

        /// <summary>
        /// تصدير قائمة الأعضاء
        /// </summary>
        Task<GeneralResponse> ExportMembersListAsync(ExportMembersRequest request);
    }
}