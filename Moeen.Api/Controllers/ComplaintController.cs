using Microsoft.AspNetCore.Mvc;
using Moeen.Api.Application;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Shared.Requests.Complaint;

namespace Moeen.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintService;

        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        #region Complaint CRUD Operations

        /// <summary>
        /// تقديم شكوى أو مقترح جديد في النظام بواسطة مستخدم
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateComplaint([FromBody] CreateComplaintRequest createComplaintRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _complaintService.CreateComplaintAsync(createComplaintRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while submitting complaint: {ex.Message}");
            }
        }

        /// <summary>
        /// تحديث حالة الشكوى (مثل: قيد المراجعة، تم الحل، مرفوضة) بواسطة لوحة التحكم الإدارية
        /// </summary>
        [HttpPut("{complaintId}/status")]
        public async Task<IActionResult> UpdateComplaintStatus(Guid complaintId, [FromBody] UpdateComplaintStatusRequest updateStatusRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _complaintService.UpdateComplaintStatusAsync(complaintId, updateStatusRequest);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while updating complaint status: {ex.Message}");
            }
        }

        /// <summary>
        /// حذف شكوى معينة من النظام
        /// </summary>
        [HttpDelete("{complaintId}")]
        public async Task<IActionResult> DeleteComplaint(Guid complaintId)
        {
            try
            {
                var result = await _complaintService.DeleteComplaintAsync(complaintId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while deleting complaint: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب تفاصيل شكوى محددة بالـ Id مع بيانات مقدمها
        /// </summary>
        [HttpGet("{complaintId}")]
        public async Task<IActionResult> GetComplaintById(Guid complaintId)
        {
            try
            {
                var result = await _complaintService.GetComplaintByIdAsync(complaintId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving complaint: {ex.Message}");
            }
        }

        /// <summary>
        /// جلب جميع الشكاوى والمقترحات الموجودة في النظام (للإدارة)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllComplaints()
        {
            try
            {
                var result = await _complaintService.GetAllComplaintsAsync();
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving complaints: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        /// <summary>
        /// جلب كافة الشكاوى والمقترحات التي قدمها مستخدم معين بواسطة الـ UserId
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetComplaintsByUserId(Guid userId)
        {
            try
            {
                var result = await _complaintService.GetComplaintsByUserIdAsync(userId);
                return result.ToActionResult();
            }
            catch (Exception ex)
            {
                return BadRequest($"An error occurred while retrieving complaints for this user: {ex.Message}");
            }
        }

        #endregion
    }
}