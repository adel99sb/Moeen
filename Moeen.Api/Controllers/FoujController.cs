using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;
using System;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,Owner,Supervisor")]
    public class FoujController : ControllerBase
    {
        private readonly IFoujService _foujService;

        public FoujController(IFoujService foujService)
        {
            _foujService = foujService;
        }

        [HttpPost]
        public async Task<ActionResult<GeneralResponse>> CreateFouj([FromBody] CreateFoujRequest request)
        {
            var response = await _foujService.CreateFoujAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{foujId:guid}")]
        public async Task<ActionResult<GeneralResponse>> UpdateFouj([FromRoute] Guid foujId, [FromBody] UpdateFoujRequest request)
        {
            request ??= new UpdateFoujRequest();
            request.FoujId = foujId;

            var response = await _foujService.UpdateFoujAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{foujId:guid}")]
        public async Task<ActionResult<GeneralResponse>> DeleteFouj([FromRoute] Guid foujId)
        {
            var response = await _foujService.DeleteFoujAsync(new DeleteFoujRequest { FoujId = foujId });
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet]
        public async Task<ActionResult<GeneralResponse>> GetAllFoujs([FromQuery] GetAllFoujsRequest request)
        {
            var response = await _foujService.GetAllFoujsAsync(request ?? new GetAllFoujsRequest());
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("{foujId:guid}/halqas")]
        public async Task<ActionResult<GeneralResponse>> AddHalqaToFouj([FromRoute] Guid foujId, [FromBody] AddHalqaToFoujRequest request)
        {
            request ??= new AddHalqaToFoujRequest();
            request.FoujId = foujId;

            var response = await _foujService.AddHalqaToFoujAsync(request);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{foujId:guid}/halqas/{halqaId:guid}")]
        public async Task<ActionResult<GeneralResponse>> RemoveHalqaFromFouj([FromRoute] Guid foujId, [FromRoute] Guid halqaId)
        {
            var response = await _foujService.RemoveHalqaFromFoujAsync(new RemoveHalqaFromFoujRequest
            {
                FoujId = foujId,
                HalqaId = halqaId
            });

            return StatusCode(response.StatusCode, response);
        }
    }
}