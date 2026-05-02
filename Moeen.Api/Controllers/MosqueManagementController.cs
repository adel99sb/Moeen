using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses.Circle;
using Moeen.Shared.Responses.CircleTeacherAssignment;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.Mosuq;

namespace Moeen.Api.Controllers
{
    [Route("api/mosques")]
    [ApiController]
    public class MosqueManagementController : ControllerBase
    {
        private readonly IMosquService _mosquService;

        public MosqueManagementController(IMosquService mosquService)
        {
            _mosquService = mosquService;
        }

        [HttpPost]
        public async Task<ActionResult<bool>> AddMosque([FromBody] AddMosquReq request)
            => Ok(await _mosquService.AddMosqu(request));

        //[HttpGet]
        //public async Task<ActionResult<GetAllMosqusResponse>> GetAllMosques()
        //    => Ok(await _mosquService.GetAllMosqus();

        [HttpGet("{mosqueId:guid}")]
        public async Task<ActionResult<MosqueDto>> GetMosqueById([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.GetMosqueByIdAsync(new GetMosqueByIdRequest { MosqueId = mosqueId }));

        [HttpGet("{mosqueId:guid}/circles")]
        public async Task<ActionResult<List<CircleDto>>> GetCirclesByMosque([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.GetCirclesByMosqueAsync(new GetCirclesByMosqueRequest { MosqueId = mosqueId }));

        [HttpGet("{mosqueId:guid}/teachers")]
        public async Task<ActionResult<List<TeacherDto>>> GetTeachersByMosque([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.GetTeachersByMosqueAsync(new GetTeachersByMosqueRequest { MosqueId = mosqueId }));

        [HttpGet("{mosqueId:guid}/statistics")]
        public async Task<ActionResult<MosqueStatisticsDto>> GetMosqueStatistics([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.GetMosqueStatisticsAsync(new GetMosqueStatisticsRequest { MosqueId = mosqueId }));

        [HttpGet("nearby")]
        public async Task<ActionResult<List<MosqueDto>>> GetNearbyMosques(
            [FromQuery] double latitude,
            [FromQuery] double longitude,
            [FromQuery] double radiusKm = 5)
            => Ok(await _mosquService.GetNearbyMosquesAsync(new GetNearbyMosquesRequest
            {
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm
            }));

        [HttpPut("{mosqueId:guid}")]
        public async Task<ActionResult<MosqueDto>> UpdateMosqueInfo([FromRoute] Guid mosqueId, [FromBody] UpdateMosqueInfoRequest request)
        {
            request.MosqueId = mosqueId;
            return Ok(await _mosquService.UpdateMosqueInfoAsync(request));
        }

        [HttpPut("{mosqueId:guid}/admin")]
        public async Task<ActionResult<OperationResponseDto>> AssignMosqueAdmin([FromRoute] Guid mosqueId, [FromBody] AssignMosqueAdminRequest request)
        {
            request.MosqueId = mosqueId;
            return Ok(await _mosquService.AssignMosqueAdminAsync(request));
        }

        [HttpDelete("{mosqueId:guid}/admin")]
        public async Task<ActionResult<OperationResponseDto>> UnassignMosqueAdmin([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.UnassignMosqueAdminAsync(new UnassignMosqueAdminRequest { MosqueId = mosqueId }));

        [HttpDelete("{mosqueId:guid}")]
        public async Task<ActionResult<OperationResponseDto>> DeleteMosque([FromRoute] Guid mosqueId)
            => Ok(await _mosquService.DeleteMosqueAsync(new DeleteMosqueRequest { MosqueId = mosqueId }));
    }
}