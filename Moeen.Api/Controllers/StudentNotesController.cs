using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.StudentNotes;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/student-notes")]
    [ApiController]
    public class StudentNotesController : ControllerBase
    {
        private readonly IStudentNotesService _studentNotesService;

        public StudentNotesController(IStudentNotesService studentNotesService)
        {
            _studentNotesService = studentNotesService;
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> AddStudentNote([FromBody] AddStudentNoteRequest request)
        {
            var response = await _studentNotesService.AddStudentNoteAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("students/{studentId:guid}")]
        public async Task<ActionResult<GeneralResponse>> GetStudentNotes(
            [FromRoute] Guid studentId,
            [FromQuery] GetStudentNotesRequest request)
        {
            request ??= new GetStudentNotesRequest();
            request.StudentId = studentId;

            var response = await _studentNotesService.GetStudentNotesAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{noteId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateStudentNote(
            [FromRoute] Guid noteId,
            [FromBody] UpdateStudentNoteRequest request)
        {
            request ??= new UpdateStudentNoteRequest();
            request.NoteId = noteId;

            var response = await _studentNotesService.UpdateStudentNoteAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{noteId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteStudentNote([FromRoute] Guid noteId)
        {
            var response = await _studentNotesService.DeleteStudentNoteAsync(noteId);
            return StatusCode(response.StatusCode, response);
        }
    }
}