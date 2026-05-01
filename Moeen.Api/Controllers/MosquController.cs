using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared;
using Moeen.Shared.Requests.Mosuq;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MosquController : ControllerBase
    {
        private readonly IMosquService _mosquService;                                                                           

        public MosquController(IMosquService mosquService)
        {
            _mosquService = mosquService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddMosque([FromBody] AddMosquReq request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.AddMosqu(request);
            return result.ToActionResult();
        }

        [HttpPost("all")]
        public async Task<IActionResult> GetAll([FromBody] GetAllMosqusRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.GetAllMosqus(request);
            return result.ToActionResult();
        }

        [HttpGet("{mosqueId:guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid mosqueId)
        {
            var result = await _mosquService.GetMosqueByIdAsync(new GetMosqueByIdRequest { MosqueId = mosqueId });
            return result.ToActionResult();
        }

        [HttpPost("circles")]
        public async Task<IActionResult> GetCircles([FromBody] GetCirclesByMosqueRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.GetCirclesByMosqueAsync(request);
            return result.ToActionResult();
        }

        [HttpPost("teachers")]
        public async Task<IActionResult> GetTeachers([FromBody] GetTeachersByMosqueRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.GetTeachersByMosqueAsync(request);
            return result.ToActionResult();
        }

        [HttpPost("statistics")]
        public async Task<IActionResult> GetStatistics([FromBody] GetMosqueStatisticsRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.GetMosqueStatisticsAsync(request);
            return result.ToActionResult();
        }

        [HttpPost("nearby")]
        public async Task<IActionResult> GetNearby([FromBody] GetNearbyMosquesRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.GetNearbyMosquesAsync(request);
            return result.ToActionResult();
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromBody] UpdateMosqueInfoRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.UpdateMosqueInfoAsync(request);
            return result.ToActionResult();
        }

        [HttpPut("assign-admin")]
        public async Task<IActionResult> AssignAdmin([FromBody] AssignMosqueAdminRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.AssignMosqueAdminAsync(request);
            return result.ToActionResult();
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete([FromBody] DeleteMosqueRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.DeleteMosqueAsync(request);
            return result.ToActionResult();
        }

        [HttpDelete("unassign-admin")]
        public async Task<IActionResult> UnassignAdmin([FromBody] UnassignMosqueAdminRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _mosquService.UnassignMosqueAdminAsync(request);
            return result.ToActionResult();
        }
    }
}