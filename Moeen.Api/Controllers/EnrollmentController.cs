using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Enrollment;
using Moeen.Api.Shared.Responses.Enrollment;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        /// <summary>
        /// تسجيل طالب جديد
        /// </summary>
        [HttpPost("register-student")]
        public async Task<ActionResult<StudentDto>> RegisterStudent(RegisterStudentRequest request)
        {
            var result = await _enrollmentService.RegisterStudentAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إضافة معلم جديد للكادر
        /// </summary>
        [HttpPost("add-teacher")]
        public async Task<ActionResult<TeacherDto>> AddTeacher(AddTeacherRequest request)
        {
            var result = await _enrollmentService.AddTeacherAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تسجيل ولي أمر جديد
        /// </summary>
        [HttpPost("register-parent")]
        public async Task<ActionResult<ParentDto>> RegisterParent(RegisterParentRequest request)
        {
            var result = await _enrollmentService.RegisterParentAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تحديث معلومات عضو
        /// </summary>
        [HttpPut("update-member")]
        public async Task<ActionResult<MemberDto>> UpdateMemberInfo(UpdateMemberInfoRequest request)
        {
            var result = await _enrollmentService.UpdateMemberInfoAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// إلغاء عضوية عضو
        /// </summary>
        [HttpPost("cancel-membership")]
        public async Task<ActionResult<bool>> CancelMembership(CancelMembershipRequest request)
        {
            var result = await _enrollmentService.CancelMembershipAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// البحث عن أعضاء
        /// </summary>
        [HttpPost("search-members")]
        public async Task<ActionResult<SearchMembersResponse>> SearchMembers(SearchMembersRequest request)
        {
            var result = await _enrollmentService.SearchMembersAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// الحصول على الملف الشخصي الكامل لعضو
        /// </summary>
        [HttpPost("member-profile")]
        public async Task<ActionResult<MemberProfileDto>> GetMemberProfile(GetMemberProfileRequest request)
        {
            var result = await _enrollmentService.GetMemberProfileAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تحديث حالة العضو
        /// </summary>
        [HttpPut("update-member-status")]
        public async Task<ActionResult<bool>> UpdateMemberStatus(UpdateMemberStatusRequest request)
        {
            var result = await _enrollmentService.UpdateMemberStatusAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// تصدير قائمة الأعضاء
        /// </summary>
        [HttpPost("export-members")]
        public async Task<ActionResult<byte[]>> ExportMembersList(ExportMembersRequest request)
        {
            var result = await _enrollmentService.ExportMembersListAsync(request);
            return File(result, "application/octet-stream", "members_export.xlsx");
        }
    }
}