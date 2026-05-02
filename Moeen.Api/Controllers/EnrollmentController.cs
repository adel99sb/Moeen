using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Responses;
using System;
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

        [HttpPost("register-student")]
        public async Task<ActionResult<GeneralResponse>> RegisterStudent([FromBody] RegisterStudentRequest request)
            => Ok(await _enrollmentService.RegisterStudentAsync(request));

        [HttpPost("add-teacher")]
        public async Task<ActionResult<GeneralResponse>> AddTeacher([FromBody] AddTeacherRequest request)
            => Ok(await _enrollmentService.AddTeacherAsync(request));

        [HttpPost("register-parent")]
        public async Task<ActionResult<GeneralResponse>> RegisterParent([FromBody] RegisterParentRequest request)
            => Ok(await _enrollmentService.RegisterParentAsync(request));

        [HttpPut("update-member")]
        public async Task<ActionResult<GeneralResponse>> UpdateMemberInfo([FromBody] UpdateMemberInfoRequest request)
            => Ok(await _enrollmentService.UpdateMemberInfoAsync(request));

        [HttpPut("students/update-info")]
        public async Task<ActionResult<GeneralResponse>> UpdateStudentInfo([FromBody] UpdateStudentInfoRequest request)
            => Ok(await _enrollmentService.UpdateStudentInfoAsync(request));

        [HttpPut("teachers/update-info")]
        public async Task<ActionResult<GeneralResponse>> UpdateTeacherInfo([FromBody] UpdateTeacherInfoRequest request)
            => Ok(await _enrollmentService.UpdateTeacherInfoAsync(request));

        [HttpPut("parents/update-info")]
        public async Task<ActionResult<GeneralResponse>> UpdateParentInfo([FromBody] UpdateParentInfoRequest request)
            => Ok(await _enrollmentService.UpdateParentInfoAsync(request));

        [HttpPost("cancel-membership")]
        public async Task<ActionResult<GeneralResponse>> CancelMembership([FromBody] CancelMembershipRequest request)
            => Ok(await _enrollmentService.CancelMembershipAsync(request));

        [HttpPost("search-members")]
        public async Task<ActionResult<GeneralResponse>> SearchMembers([FromBody] SearchMembersRequest request)
            => Ok(await _enrollmentService.SearchMembersAsync(request));

        [HttpGet("search-members")]
        public async Task<ActionResult<GeneralResponse>> SearchMembersGet(
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
        public async Task<ActionResult<GeneralResponse>> GetMemberProfile([FromBody] GetMemberProfileRequest request)
            => Ok(await _enrollmentService.GetMemberProfileAsync(request));

        [HttpGet("members/{memberId}")]
        public async Task<ActionResult<GeneralResponse>> GetMemberProfileGet([FromRoute] string memberId)
            => Ok(await _enrollmentService.GetMemberProfileAsync(new GetMemberProfileRequest { MemberId = memberId }));

        [HttpPut("update-member-status")]
        public async Task<ActionResult<GeneralResponse>> UpdateMemberStatus([FromBody] UpdateMemberStatusRequest request)
            => Ok(await _enrollmentService.UpdateMemberStatusAsync(request));

        [HttpGet("students")]
        public async Task<ActionResult<GeneralResponse>> GetAllStudents([FromQuery] GetAllStudentsRequest request)
            => Ok(await _enrollmentService.GetAllStudentsAsync(request));

        [HttpGet("teachers")]
        public async Task<ActionResult<GeneralResponse>> GetAllTeachers([FromQuery] GetAllTeachersRequest request)
            => Ok(await _enrollmentService.GetAllTeachersAsync(request));

        [HttpGet("parents")]
        public async Task<ActionResult<GeneralResponse>> GetAllParents([FromQuery] GetAllParentsRequest request)
            => Ok(await _enrollmentService.GetAllParentsAsync(request));

        [HttpGet("supervisors")]
        public async Task<ActionResult<GeneralResponse>> GetAllSupervisors([FromQuery] GetAllSupervisorsRequest request)
            => Ok(await _enrollmentService.GetAllSupervisorsAsync(request));

        [HttpGet("parents/{parentId:guid}/children")]
        public async Task<ActionResult<GeneralResponse>> GetChildrenByParent([FromRoute] Guid parentId)
            => Ok(await _enrollmentService.GetChildrenByParentAsync(new GetChildrenByParentRequest { ParentId = parentId }));

        [HttpGet("statistics")]
        public async Task<ActionResult<GeneralResponse>> GetMemberStatistics([FromQuery] GetMemberStatisticsRequest request)
            => Ok(await _enrollmentService.GetMemberStatisticsAsync(request));

        [HttpDelete("students/{studentId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteStudent([FromRoute] Guid studentId)
            => Ok(await _enrollmentService.DeleteStudentAsync(new DeleteStudentRequest { StudentId = studentId }));

        [HttpDelete("teachers/{teacherId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteTeacher([FromRoute] Guid teacherId)
            => Ok(await _enrollmentService.DeleteTeacherAsync(new DeleteTeacherRequest { TeacherId = teacherId }));

        [HttpDelete("parents/{parentId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteParent([FromRoute] Guid parentId)
            => Ok(await _enrollmentService.DeleteParentAsync(new DeleteParentRequest { ParentId = parentId }));

        [HttpPost("export-members")]
        public async Task<ActionResult<GeneralResponse>> ExportMembersList([FromBody] ExportMembersRequest request)
            => Ok(await _enrollmentService.ExportMembersListAsync(request));
    }
}