using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IExamHalqaService
    {
        Task<GeneralResponse> CreateExamTeacherAsync(CreateExamTeacherRequest request);
        Task<GeneralResponse> GetExamTeacherByIdAsync(GetExamTeacherByIdRequest request);
        Task<GeneralResponse> AssignHalqaToExamTeacherAsync(AssignHalqaToExamTeacherRequest request);
    }
}