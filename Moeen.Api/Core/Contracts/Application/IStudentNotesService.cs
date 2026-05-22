using Moeen.Shared.Requests.StudentNotes;
using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IStudentNotesService
    {
        Task<GeneralResponse> AddStudentNoteAsync(AddStudentNoteRequest request);
        Task<GeneralResponse> GetStudentNotesAsync(GetStudentNotesRequest request);
        Task<GeneralResponse> UpdateStudentNoteAsync(UpdateStudentNoteRequest request);
        Task<GeneralResponse> DeleteStudentNoteAsync(Guid noteId);
    }
}