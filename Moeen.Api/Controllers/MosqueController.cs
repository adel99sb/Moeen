using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Mosque;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MosqueController : ControllerBase
    {
        private readonly IMosqueService _mosqueService;

        public MosqueController(IMosqueService mosqueService)
        {
            _mosqueService = mosqueService;
        }

        #region Mosque CRUD Operations

        /// <summary>
        /// إنشاء جامع جديد في النظام
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateMosque([FromBody] CreateMosqueRequest createMosqueRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mosqueService.CreateMosqueAsync(createMosqueRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while creating mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// تعديل بيانات جامع معين بواسطة المعرف الرقمي (Guid)
        /// </summary>
        [HttpPut("{mosqueId}")]
        public async Task<IActionResult> UpdateMosque(Guid mosqueId, [FromBody] CreateMosqueRequest updateMosqueRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mosqueService.UpdateMosqueAsync(mosqueId, updateMosqueRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف جامع نهائياً من النظام
        /// </summary>
        [HttpDelete("{mosqueId}")]
        public async Task<IActionResult> DeleteMosque(Guid mosqueId)
        {
            try
            {
                var result = await _mosqueService.DeleteMosqueAsync(mosqueId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب بيانات جامع محدد عن طريق الـ Id
        /// </summary>
        [HttpGet("{mosqueId}")]
        public async Task<IActionResult> GetMosqueById(Guid mosqueId)
        {
            try
            {
                var result = await _mosqueService.GetMosqueByIdAsync(mosqueId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب قائمة بكافة المساجد المسجلة في النظام
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllMosques()
        {
            try
            {
                var result = await _mosqueService.GetAllMosquesAsync();
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving mosques: {ex.Message}");
            }
        }

        #endregion

        #region Mosque Users Management

        /// <summary>
        /// ربط وإضافة مستخدم إلى جامع معين
        /// </summary>
        [HttpPost("add-user")]
        public async Task<IActionResult> AddUserToMosque([FromBody] AddUserToMosqueRequest addUserToMosqueRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _mosqueService.AddUserToMosqueAsync(addUserToMosqueRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while adding user to mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// إلغاء ربط مستخدم وحذفه من قائمة مستخدمي الجامع
        /// </summary>
        [HttpDelete("{mosqueId}/users/{userId}")]
        public async Task<IActionResult> RemoveUserFromMosque(Guid mosqueId, Guid userId)
        {
            try
            {
                var result = await _mosqueService.RemoveUserFromMosqueAsync(mosqueId, userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while removing user from mosque: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب قائمة بكافة المستخدمين المرتبطين بجامع محدد
        /// </summary>
        [HttpGet("{mosqueId}/users")]
        public async Task<IActionResult> GetMosqueUsers(Guid mosqueId)
        {
            try
            {
                var result = await _mosqueService.GetMosqueUsersAsync(mosqueId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving mosque users: {ex.Message}");
            }
        }

        #endregion
    }
}