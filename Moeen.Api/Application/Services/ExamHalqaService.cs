using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Exam_Halqa;
using Moeen.Api.Shared.Responses.Exam_Halqa;

namespace Moeen.Api.Application.Services
{
    public class ExamHalqaService : IExamHalqaService
    {
            public async Task<CreateExamTeacherResponse> CreateAsync(CreateExamTeacherRequest request)
            {
                var response = new CreateExamTeacherResponse();

                // TODO: Add logic here

                return response;
            }

        public async Task<GetExamTeacherByIdResponse> GetByIdAsync(GetExamTeacherByIdRequest request)
            {
                var response = new GetExamTeacherByIdResponse();

                // TODO: Add logic here

                return response;
            }

            public async Task<AssignHalqaResponse> AssignHalqaAsync(AssignHalqaToExamTeacherRequest request)
            {
                var response = new AssignHalqaResponse();

                // TODO: Add logic here

                return response;
            }
        
    }
}