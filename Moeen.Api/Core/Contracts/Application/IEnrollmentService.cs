using Moeen.Api.Shared.Requests.Enrollment;
using Moeen.Api.Shared.Responses.Enrollment;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IEnrollmentService
    {
        /// <summary>
        /// تسجيل طالب جديد
        /// </summary>
        /// <param name="request">بيانات الطالب للتسجيل</param>
        /// <returns>بيانات الطالب المسجل</returns>
        Task<StudentDto> RegisterStudentAsync(RegisterStudentRequest request);

        /// <summary>
        /// إضافة معلم جديد للكادر
        /// </summary>
        /// <param name="request">بيانات المعلم للإضافة</param>
        /// <returns>بيانات المعلم المضاف</returns>
        Task<TeacherDto> AddTeacherAsync(AddTeacherRequest request);

        /// <summary>
        /// تسجيل ولي أمر جديد
        /// </summary>
        /// <param name="request">بيانات ولي الأمر للتسجيل</param>
        /// <returns>بيانات ولي الأمر المسجل</returns>
        Task<ParentDto> RegisterParentAsync(RegisterParentRequest request);

        /// <summary>
        /// تحديث معلومات عضو (طالب، معلم، ولي أمر)
        /// </summary>
        /// <param name="request">معرف العضو والبيانات المراد تحديثها</param>
        /// <returns>بيانات العضو بعد التحديث</returns>
        Task<MemberDto> UpdateMemberInfoAsync(UpdateMemberInfoRequest request);

        /// <summary>
        /// إلغاء عضوية عضو (إنهاء العضوية)
        /// </summary>
        /// <param name="request">معرف العضو وسبب الإلغاء</param>
        /// <returns>true إذا تم الإلغاء بنجاح</returns>
        Task<bool> CancelMembershipAsync(CancelMembershipRequest request);

        /// <summary>
        /// البحث عن أعضاء بناءً على معايير محددة
        /// </summary>
        /// <param name="request">معايير البحث (الاسم، البريد، النوع، ...)</param>
        /// <returns>قائمة الأعضاء مع معلومات التصفح</returns>
        Task<SearchMembersResponse> SearchMembersAsync(SearchMembersRequest request);

        /// <summary>
        /// الحصول على الملف الشخصي الكامل لعضو معين
        /// </summary>
        /// <param name="request">معرف العضو</param>
        /// <returns>الملف الشخصي المفصل</returns>
        Task<MemberProfileDto> GetMemberProfileAsync(GetMemberProfileRequest request);

        /// <summary>
        /// تحديث حالة العضو (نشط، موقوف، متخرج)
        /// </summary>
        /// <param name="request">معرف العضو والحالة الجديدة</param>
        /// <returns>true إذا تم التحديث بنجاح</returns>
        Task<bool> UpdateMemberStatusAsync(UpdateMemberStatusRequest request);

        /// <summary>
        /// تصدير قائمة الأعضاء إلى ملف (Excel, PDF, CSV)
        /// </summary>
        /// <param name="request">معايير التصدير (الصيغة، الحقول، التصفية)</param>
        /// <returns>محتوى الملف كـ byte[] مع معلومات إضافية (اختياري)</returns>
        Task<byte[]> ExportMembersListAsync(ExportMembersRequest request); // أو Task<ExportMembersResponse>
    }
}