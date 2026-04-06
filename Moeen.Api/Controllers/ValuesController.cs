using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Core.Constants;
using System.Runtime.InteropServices;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        [HttpGet("Test")]
        public async Task<IActionResult> Test()
        {
            return Ok("Test Secs");
        }
        [Authorize]
        [HttpGet("TestAuth")]
        public async Task<IActionResult> TestAuth()
        {
            return Ok("Auth Secs");
        }
        [Authorize(Roles = nameof(Roles.Student))]
        [HttpGet("TestRole")]
        public async Task<IActionResult> TestRole()
        {
            return Ok("Auth with role Secs");
        }
    }
}
