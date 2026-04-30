using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Moeen.Shared.Constants;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
            /// <summary>
        /// GET: فحص بسيط لعمل الـ API.
        /// </summary>
        [HttpGet("test")]
        public IActionResult Test() => Ok("Test Secs");

        /// <summary>
        /// GET: فحص endpoint محمي بمصادقة.
        /// </summary>
        [Authorize]
        [HttpGet("test-auth")]
        public IActionResult TestAuth() => Ok("Auth Secs");

        /// <summary>
        /// GET: فحص endpoint محمي بدور Student.
        /// </summary>
        [Authorize(Roles = nameof(Roles.Student))]
        [HttpGet("test-role")]
        public IActionResult TestRole() => Ok("Auth with role Secs");
    }
}
