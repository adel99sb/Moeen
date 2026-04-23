using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Mosuq;
using Moeen.Api.Shared.Responses.Circle;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.Enrollment;
using Moeen.Api.Shared.Responses.Mosuq;

namespace Moeen.Api.Application.Services
{
    public class MosquService : IMosquService
    {
        public Task<bool> AddMosqu(AddMosquReq req)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> AssignMosqueAdminAsync(AssignMosqueAdminRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<GetAllMosqusResponse> GetAllMosqus()
        {
            throw new NotImplementedException();
        }

        public Task<List<CircleDto>> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MosqueDto> GetMosqueByIdAsync(GetMosqueByIdRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MosqueStatisticsDto> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<MosqueDto>> GetNearbyMosquesAsync(GetNearbyMosquesRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<List<TeacherDto>> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<OperationResponseDto> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<MosqueDto> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request)
        {
            throw new NotImplementedException();
        }
    }
}

