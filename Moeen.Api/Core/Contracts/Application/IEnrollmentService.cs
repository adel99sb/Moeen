using Moeen.Api.Shared.Requests.Enrollment;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Enrollment;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IEnrollmentService
    {
        /// <summary>
        /// تسجيل طالب جديد
        /// </summary>
        Task<StudentDto> RegisterStudentAsync(RegisterStudentRequest request);

        /// <summary>
        /// إضافة معلم جديد للكادر
        /// </summary>
        Task<TeacherDto> AddTeacherAsync(AddTeacherRequest request);

        /// <summary>
        /// تسجيل ولي أمر جديد
        /// </summary>
        Task<ParentDto> RegisterParentAsync(RegisterParentRequest request);

        /// <summary>
        /// تحديث معلومات عضو (عام)
        /// </summary>
        Task<MemberDto> UpdateMemberInfoAsync(UpdateMemberInfoRequest request);

        /// <summary>
        /// إلغاء عضوية عضو (إنهاء العضوية)
        /// </summary>
        Task<bool> CancelMembershipAsync(CancelMembershipRequest request);

        /// <summary>
        /// البحث عن أعضاء بناءً على معايير محددة
        /// </summary>
        Task<SearchMembersResponse> SearchMembersAsync(SearchMembersRequest request);

        /// <summary>
        /// الحصول على الملف الشخصي الكامل لعضو معين
        /// </summary>
        Task<MemberProfileDto> GetMemberProfileAsync(GetMemberProfileRequest request);

        /// <summary>
        /// تحديث حالة العضو (نشط، موقوف، متخرج)
        /// </summary>
        Task<bool> UpdateMemberStatusAsync(UpdateMemberStatusRequest request);

        /// <summary>
        /// تصدير قائمة الأعضاء إلى ملف
        /// </summary>
        Task<byte[]> ExportMembersListAsync(ExportMembersRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع الطلاب مع التصفح والتصفية
        /// </summary>
        Task<PagedList<StudentDto>> GetAllStudentsAsync(GetAllStudentsRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع المعلمين مع التصفح والتصفية
        /// </summary>
        Task<PagedList<TeacherDto>> GetAllTeachersAsync(GetAllTeachersRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع أولياء الأمور مع التصفح والتصفية
        /// </summary>
        Task<PagedList<ParentDto>> GetAllParentsAsync(GetAllParentsRequest request);

        /// <summary>
        /// [GET] جلب قائمة جميع المشرفين (لواجهة المالك)
        /// </summary>
        Task<PagedList<SupervisorDto>> GetAllSupervisorsAsync(GetAllSupervisorsRequest request);

        /// <summary>
        /// [GET] جلب الأبناء المرتبطين بولي أمر معين
        /// </summary>
        Task<List<StudentDto>> GetChildrenByParentAsync(GetChildrenByParentRequest request);

        /// <summary>
        /// [GET] إحصائيات الأعضاء (حسب النوع/الحالة/المسجد)
        /// </summary>
        Task<MemberStatisticsDto> GetMemberStatisticsAsync(GetMemberStatisticsRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات طالب محددة
        /// </summary>
        Task<StudentDto> UpdateStudentInfoAsync(UpdateStudentInfoRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات معلم محددة
        /// </summary>
        Task<TeacherDto> UpdateTeacherInfoAsync(UpdateTeacherInfoRequest request);

        /// <summary>
        /// [PUT] تحديث معلومات ولي أمر محددة
        /// </summary>
        Task<ParentDto> UpdateParentInfoAsync(UpdateParentInfoRequest request);

        /// <summary>
        /// [DELETE] حذف سجل طالب نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteStudentAsync(DeleteStudentRequest request);

        /// <summary>
        /// [DELETE] حذف سجل معلم نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteTeacherAsync(DeleteTeacherRequest request);

        /// <summary>
        /// [DELETE] حذف سجل ولي أمر نهائيًا
        /// </summary>
        Task<OperationResponseDto> DeleteParentAsync(DeleteParentRequest request);
    }
}