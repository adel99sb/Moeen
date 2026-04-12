using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Enrollment;
using Moeen.Api.Shared.Responses.Enrollment;

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

        [HttpPost("register-student")]
        public async Task<ActionResult<StudentDto>> RegisterStudent(RegisterStudentRequest request)
            => Ok(await _enrollmentService.RegisterStudentAsync(request));

        [HttpPost("add-teacher")]
        public async Task<ActionResult<TeacherDto>> AddTeacher(AddTeacherRequest request)
            => Ok(await _enrollmentService.AddTeacherAsync(request));

        [HttpPost("register-parent")]
        public async Task<ActionResult<ParentDto>> RegisterParent(RegisterParentRequest request)
            => Ok(await _enrollmentService.RegisterParentAsync(request));

        [HttpPut("update-member")]
        public async Task<ActionResult<MemberDto>> UpdateMemberInfo(UpdateMemberInfoRequest request)
            => Ok(await _enrollmentService.UpdateMemberInfoAsync(request));

        [HttpPost("cancel-membership")]
        public async Task<ActionResult<bool>> CancelMembership(CancelMembershipRequest request)
            => Ok(await _enrollmentService.CancelMembershipAsync(request));

        /// <summary>
        /// POST (قديم/متوافق): بحث متقدم بالأعضاء.
        /// </summary>
        [HttpPost("search-members")]
        public async Task<ActionResult<SearchMembersResponse>> SearchMembers(SearchMembersRequest request)
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

        [HttpPost("member-profile")]
        public async Task<ActionResult<MemberProfileDto>> GetMemberProfile(GetMemberProfileRequest request)
            => Ok(await _enrollmentService.GetMemberProfileAsync(request));

        /// <summary>
        /// GET (جديد): ملف عضو عبر Route.
        /// </summary>
        [HttpGet("members/{memberId}")]
        public async Task<ActionResult<MemberProfileDto>> GetMemberProfileGet([FromRoute] string memberId)
            => Ok(await _enrollmentService.GetMemberProfileAsync(new GetMemberProfileRequest { MemberId = memberId }));

        [HttpPut("update-member-status")]
        public async Task<ActionResult<bool>> UpdateMemberStatus(UpdateMemberStatusRequest request)
            => Ok(await _enrollmentService.UpdateMemberStatusAsync(request));

        [HttpPost("export-members")]
        public async Task<ActionResult<byte[]>> ExportMembersList(ExportMembersRequest request)
        {
            var result = await _enrollmentService.ExportMembersListAsync(request);
            return File(result, "application/octet-stream", "members_export.xlsx");
        }
    }
}   