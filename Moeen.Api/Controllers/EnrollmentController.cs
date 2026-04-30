using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Enrollment;
using System.Collections.Generic;
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
        /// أمر: تسجيل طالب جديد.
        /// </summary>
        [HttpPost("register-student")]
        public async Task<ActionResult<StudentDto>> RegisterStudent([FromBody] RegisterStudentRequest request)
            => Ok(await _enrollmentService.RegisterStudentAsync(request));

        /// <summary>
        /// أمر: إضافة معلم جديد.
        /// </summary>
        [HttpPost("add-teacher")]
        public async Task<ActionResult<TeacherDto>> AddTeacher([FromBody] AddTeacherRequest request)
            => Ok(await _enrollmentService.AddTeacherAsync(request));

        /// <summary>
        /// أمر: تسجيل ولي أمر جديد.
        /// </summary>
        [HttpPost("register-parent")]
        public async Task<ActionResult<ParentDto>> RegisterParent([FromBody] RegisterParentRequest request)
            => Ok(await _enrollmentService.RegisterParentAsync(request));

        /// <summary>
        /// PUT (قديم/متوافق): تحديث معلومات عضو بشكل عام.
        /// </summary>
        [HttpPut("update-member")]
        public async Task<ActionResult<MemberDto>> UpdateMemberInfo([FromBody] UpdateMemberInfoRequest request)
            => Ok(await _enrollmentService.UpdateMemberInfoAsync(request));

        /// <summary>
        /// PUT: تحديث معلومات طالب متخصصة.
        /// </summary>
        [HttpPut("students/update-info")]
        public async Task<ActionResult<StudentDto>> UpdateStudentInfo([FromBody] UpdateStudentInfoRequest request)
            => Ok(await _enrollmentService.UpdateStudentInfoAsync(request));

        /// <summary>
        /// PUT: تحديث معلومات معلم متخصصة.
        /// </summary>
        [HttpPut("teachers/update-info")]
        public async Task<ActionResult<TeacherDto>> UpdateTeacherInfo([FromBody] UpdateTeacherInfoRequest request)
            => Ok(await _enrollmentService.UpdateTeacherInfoAsync(request));

        /// <summary>
        /// PUT: تحديث معلومات ولي أمر متخصصة.
        /// </summary>
        [HttpPut("parents/update-info")]
        public async Task<ActionResult<ParentDto>> UpdateParentInfo([FromBody] UpdateParentInfoRequest request)
            => Ok(await _enrollmentService.UpdateParentInfoAsync(request));

        /// <summary>
        /// أمر: إلغاء عضوية عضو.
        /// </summary>
        [HttpPost("cancel-membership")]
        public async Task<ActionResult<bool>> CancelMembership([FromBody] CancelMembershipRequest request)
            => Ok(await _enrollmentService.CancelMembershipAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم بالأعضاء.
        /// </summary>
        [HttpPost("search-members")]
        public async Task<ActionResult<SearchMembersResponse>> SearchMembers([FromBody] SearchMembersRequest request)
            => Ok(await _enrollmentService.SearchMembersAsync(request));

        /// <summary>
        /// GET (جديد): بحث سريع بالأعضاء.
        /// </summary>
        [HttpGet("search-members")]
        public async Task<ActionResult<SearchMembersResponse>> SearchMembersGet(
            [FromQuery] string? name,
            [FromQuery] string? email,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var request = new SearchMembersRequest
            {
                Name = name ?? string.Empty,
                Email = email ?? string.Empty,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            return Ok(await _enrollmentService.SearchMembersAsync(request));
        }

        /// <summary>
        /// POST (قديم/متوافق): جلب الملف الشخصي لعضو.
        /// </summary>
        [HttpPost("member-profile")]
        public async Task<ActionResult<MemberProfileDto>> GetMemberProfile([FromBody] GetMemberProfileRequest request)
            => Ok(await _enrollmentService.GetMemberProfileAsync(request));

        /// <summary>
        /// GET (جديد): جلب ملف عضو عبر Route.
        /// </summary>
        [HttpGet("members/{memberId}")]
        public async Task<ActionResult<MemberProfileDto>> GetMemberProfileGet([FromRoute] string memberId)
            => Ok(await _enrollmentService.GetMemberProfileAsync(new GetMemberProfileRequest { MemberId = memberId }));

        /// <summary>
        /// PUT: تحديث حالة العضو.
        /// </summary>
        [HttpPut("update-member-status")]
        public async Task<ActionResult<bool>> UpdateMemberStatus([FromBody] UpdateMemberStatusRequest request)
            => Ok(await _enrollmentService.UpdateMemberStatusAsync(request));

        /// <summary>
        /// GET: جلب جميع الطلاب مع التصفح والتصفية.
        /// </summary>
        [HttpGet("students")]
        public async Task<ActionResult<PagedList<StudentDto>>> GetAllStudents([FromQuery] GetAllStudentsRequest request)
            => Ok(await _enrollmentService.GetAllStudentsAsync(request));

        /// <summary>
        /// GET: جلب جميع المعلمين مع التصفح والتصفية.
        /// </summary>
        [HttpGet("teachers")]
        public async Task<ActionResult<PagedList<TeacherDto>>> GetAllTeachers([FromQuery] GetAllTeachersRequest request)
            => Ok(await _enrollmentService.GetAllTeachersAsync(request));

        /// <summary>
        /// GET: جلب جميع أولياء الأمور مع التصفح والتصفية.
        /// </summary>
        [HttpGet("parents")]
        public async Task<ActionResult<PagedList<ParentDto>>> GetAllParents([FromQuery] GetAllParentsRequest request)
            => Ok(await _enrollmentService.GetAllParentsAsync(request));

        /// <summary>
        /// GET: جلب جميع المشرفين (لاستخدام واجهة المالك).
        /// </summary>
        [HttpGet("supervisors")]
        public async Task<ActionResult<PagedList<SupervisorDto>>> GetAllSupervisors([FromQuery] GetAllSupervisorsRequest request)
            => Ok(await _enrollmentService.GetAllSupervisorsAsync(request));

        /// <summary>
        /// GET: جلب الأبناء المرتبطين بولي أمر.
        /// </summary>
        [HttpGet("parents/{parentId:guid}/children")]
        public async Task<ActionResult<List<StudentDto>>> GetChildrenByParent([FromRoute] Guid parentId)
            => Ok(await _enrollmentService.GetChildrenByParentAsync(new GetChildrenByParentRequest { ParentId = parentId }));

        /// <summary>
        /// GET: إحصائيات الأعضاء.
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<MemberStatisticsDto>> GetMemberStatistics([FromQuery] GetMemberStatisticsRequest request)
            => Ok(await _enrollmentService.GetMemberStatisticsAsync(request));

        /// <summary>
        /// DELETE: حذف سجل طالب نهائيًا.
        /// </summary>
        [HttpDelete("students/{studentId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteStudent([FromRoute] Guid studentId)
            => Ok(await _enrollmentService.DeleteStudentAsync(new DeleteStudentRequest { StudentId = studentId }));

        /// <summary>
        /// DELETE: حذف سجل معلم نهائيًا.
        /// </summary>
        [HttpDelete("teachers/{teacherId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteTeacher([FromRoute] Guid teacherId)
            => Ok(await _enrollmentService.DeleteTeacherAsync(new DeleteTeacherRequest { TeacherId = teacherId }));

        /// <summary>
        /// DELETE: حذف سجل ولي أمر نهائيًا.
        /// </summary>
        [HttpDelete("parents/{parentId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteParent([FromRoute] Guid parentId)
            => Ok(await _enrollmentService.DeleteParentAsync(new DeleteParentRequest { ParentId = parentId }));

        /// <summary>
        /// POST (قديم/متوافق): تصدير قائمة الأعضاء.
        /// </summary>
        [HttpPost("export-members")]
        public async Task<ActionResult<byte[]>> ExportMembersList([FromBody] ExportMembersRequest request)
        {
            var result = await _enrollmentService.ExportMembersListAsync(request);
            return File(result, "application/octet-stream", "members_export.xlsx");
        }
    }
}