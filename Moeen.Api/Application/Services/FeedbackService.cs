using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Moeen.Api.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<User> _userManager;

        public FeedbackService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        /// <summary>
        /// استقبال شكوى جديدة
        /// </summary>
        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            if (request?.ComplaintData == null)
                return GeneralResponse.BadRequest("بيانات الشكوى مطلوبة.");

            // استخدام UserId الحالي لمنع انتحال الهوية
            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول.");

            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId.Value,
                content = request.ComplaintData.Content,
                created_at = DateTime.UtcNow,
                Type = FeedbackType.Complaint,
                Status = ComplaintStatus.InProgress
            };

            await _unitOfWork.Repository<Complaint>().AddAsync(complaint);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تقديم الشكوى بنجاح.");
        }

        /// <summary>
        /// استقبال اقتراح جديد
        /// </summary>
        public async Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            if (request?.SuggestionData == null)
                return GeneralResponse.BadRequest("بيانات الاقتراح مطلوبة.");

            // استخدام UserId الحالي لمنع انتحال الهوية
            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول.");

            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId.Value,
                Title = request.SuggestionData.Title,
                content = request.SuggestionData.Content,
                created_at = DateTime.UtcNow,
                Type = FeedbackType.Suggestion,
                SuggestionStatus = SuggestionStatus.InProgress
            };

            await _unitOfWork.Repository<Complaint>().AddAsync(complaint);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تقديم الاقتراح بنجاح.");
        }

        /// <summary>
        /// إدارة الشكاوى والاقتراحات (الرد عليها)
        /// </summary>
        public async Task<GeneralResponse> ManageFeedbacksAsync(ManageFeedbackRequest request)
        {
            if (request?.Id == Guid.Empty || string.IsNullOrWhiteSpace(request.Response))
                return GeneralResponse.BadRequest("معرف الشكوى والرد مطلوبان.");

            // تحقق من صلاحيات المدير/مشرف
            if (!await IsSupervisorOrAdminAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بالإدارة.");

            var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.Id);
            if (complaint == null)
                return GeneralResponse.NotFound("الfeedback غير موجود.");

            complaint.Response = request.Response;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم الرد على الfeedback بنجاح.");
        }

        /// <summary>
        /// تحديث حالة الشكوى
        /// </summary>
        public async Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request)
        {
            if (request?.ComplaintId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الشكوى مطلوب.");

            // تحقق من صلاحيات المدير/مشرف
            if (!await IsSupervisorOrAdminAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحديث الحالة.");

            var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.ComplaintId);
            if (complaint == null || complaint.Type != FeedbackType.Complaint)
                return GeneralResponse.NotFound("الشكوى غير موجودة.");

            complaint.Status = request.Status;
            complaint.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Note))
                complaint.Response = request.Note;

            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تحديث حالة الشكوى بنجاح.");
        }

        /// <summary>
        /// استرجاع قائمة الشكاوى مع ترقيم
        /// </summary>
        public async Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var query = _unitOfWork.Repository<Complaint>().GetAllQueryable()
                .AsNoTracking()
                .Where(c => c.Type == FeedbackType.Complaint);

            var total = await query.CountAsync();
            var complaints = await query
                .OrderByDescending(c => c.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.Title,
                    Content = c.content,
                    CreatedAt = c.created_at,
                    c.Type,
                    c.Status,
                    c.SuggestionStatus,
                    c.Response,
                    c.UpdatedAt
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم استرجاع الشكاوى بنجاح.", complaints, page, pageSize, total);
        }

        /// <summary>
        /// استرجاع قائمة الاقتراحات مع ترقيم
        /// </summary>
        public async Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request)
        {
            var page = request.Page > 0 ? request.Page : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var query = _unitOfWork.Repository<Complaint>().GetAllQueryable()
                .AsNoTracking()
                .Where(c => c.Type == FeedbackType.Suggestion);

            var total = await query.CountAsync();
            var suggestions = await query
                .OrderByDescending(c => c.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.UserId,
                    c.Title,
                    Content = c.content,
                    CreatedAt = c.created_at,
                    c.Type,
                    c.Status,
                    c.SuggestionStatus,
                    c.Response,
                    c.UpdatedAt
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم استرجاع الاقتراحات بنجاح.", suggestions, page, pageSize, total);
        }

        /// <summary>
        /// تحديث حالة الاقتراح
        /// </summary>
        public async Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request)
        {
            if (request?.SuggestionId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الاقتراح مطلوب.");

            // تحقق من صلاحيات المدير/مشرف
            if (!await IsSupervisorOrAdminAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحديث الحالة.");

            var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.SuggestionId);
            if (complaint == null || complaint.Type != FeedbackType.Suggestion)
                return GeneralResponse.NotFound("الاقتراح غير موجود.");

            complaint.SuggestionStatus = request.Status;
            complaint.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تحديث حالة الاقتراح بنجاح.");
        }

        private async Task<bool> IsSupervisorOrAdminAsync()
        {
            var userId = _currentUserService.CurrentUserId;
            if (!userId.HasValue) return false;

            var user = await _userManager.FindByIdAsync(userId.Value.ToString());
            if (user == null) return false;

            var roles = await _userManager.GetRolesAsync(user);
            return roles.Contains("Admin") || roles.Contains("Supervisor");
        }
        public async Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الشكوى مطلوب.");

            // تحقق من صلاحيات المدير/مشرف
            if (!await IsSupervisorOrAdminAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بالحذف.");

            var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(complaintId);
            if (complaint == null)
                return GeneralResponse.NotFound("الشكوى أو الاقتراح غير موجود.");

            await _unitOfWork.Repository<Complaint>().DeleteAsync(complaint);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف الشكوى/الاقتراح بنجاح.");
        }
    }
}
