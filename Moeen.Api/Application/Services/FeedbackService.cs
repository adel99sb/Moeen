using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Feedback;
using Moeen.Shared.Responses;

namespace Moeen.Api.Application.Services
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public FeedbackService(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> SubmitComplaintAsync(SubmitComplaintRequest request)
        {
            if (request?.ComplaintData == null)
                return GeneralResponse.BadRequest("بيانات الشكوى مطلوبة.");

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

        public async Task<GeneralResponse> SubmitSuggestionAsync(SubmitSuggestionRequest request)
        {
            if (request?.SuggestionData == null)
                return GeneralResponse.BadRequest("بيانات الاقتراح مطلوبة.");

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return GeneralResponse.Unauthorized("يجب تسجيل الدخول.");

            var suggestion = new Complaint
            {
                Id = Guid.NewGuid(),
                UserId = currentUserId.Value,
                Title = request.SuggestionData.Title,
                content = request.SuggestionData.Content,
                created_at = DateTime.UtcNow,
                Type = FeedbackType.Suggestion,
                SuggestionStatus = SuggestionStatus.InProgress
            };

            await _unitOfWork.Repository<Complaint>().AddAsync(suggestion);
            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم تقديم الاقتراح بنجاح.");
        }

        public async Task<GeneralResponse> ManageFeedbacksAsync(ManageFeedbackRequest request)
        {
            if (request?.Id == Guid.Empty || string.IsNullOrWhiteSpace(request.Response))
                return GeneralResponse.BadRequest("معرف الشكوى والرد مطلوبان.");

            if (!CanSupervisorManage())
                return GeneralResponse.Unauthorized("غير مصرح لك بالإدارة.");

            var feedback = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.Id);
            if (feedback == null)
                return GeneralResponse.NotFound("الشكوى أو الاقتراح غير موجود.");

            if (IsSupervisorOnly() && IsTransferredToOwner(feedback))
                return GeneralResponse.Unauthorized("تم تحويل هذا السجل إلى المالك ولم يعد متاحاً للمشرف.");

            feedback.Response = request.Response.Trim();
            feedback.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم حفظ الرد بنجاح.");
        }

        public async Task<GeneralResponse> UpdateComplaintStatusAsync(UpdateComplaintStatusRequest request)
        {
            if (request?.ComplaintId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الشكوى مطلوب.");

            if (!CanSupervisorManage())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحديث الحالة.");

            if (request.Status == ComplaintStatus.TransferredToOwner)
                return GeneralResponse.BadRequest("استخدم عملية تحويل للمالك بدلاً من تعديل الحالة مباشرة.");

            var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.ComplaintId);
            if (complaint == null || complaint.Type != FeedbackType.Complaint)
                return GeneralResponse.NotFound("الشكوى غير موجودة.");

            if (IsSupervisorOnly() && IsTransferredToOwner(complaint))
                return GeneralResponse.Unauthorized("تم تحويل هذه الشكوى إلى المالك.");

            complaint.Status = request.Status;
            complaint.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(request.Note))
                complaint.Response = request.Note.Trim();

            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم تحديث حالة الشكوى بنجاح.");
        }

        public async Task<GeneralResponse> GetComplaintsAsync(PaginationRequest request)
        {
            if (!CanReadManagementFeedback())
                return GeneralResponse.Unauthorized("غير مصرح لك بعرض الشكاوى.");

            var page = request?.Page > 0 ? request.Page : 1;
            var pageSize = request?.PageSize > 0 ? request.PageSize : 20;

            var query = _unitOfWork.Repository<Complaint>().GetAllQueryable()
                .AsNoTracking()
                .Where(c => c.Type == FeedbackType.Complaint);

            if (IsOwnerOnly())
                query = query.Where(c => c.Status == ComplaintStatus.TransferredToOwner);
            else if (IsSupervisorOnly())
                query = query.Where(c => c.Status != ComplaintStatus.TransferredToOwner);

            var total = await query.CountAsync();
            var complaints = await query
                .OrderByDescending(c => c.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ComplaintDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Title = c.Title,
                    Content = c.content,
                    CreatedAt = c.created_at,
                    Type = c.Type,
                    Status = c.Status,
                    SuggestionStatus = c.SuggestionStatus,
                    Response = c.Response,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم استرجاع الشكاوى بنجاح.", complaints, page, pageSize, total);
        }

        public async Task<GeneralResponse> GetSuggestionsAsync(PaginationRequest request)
        {
            if (!CanReadManagementFeedback())
                return GeneralResponse.Unauthorized("غير مصرح لك بعرض الاقتراحات.");

            var page = request?.Page > 0 ? request.Page : 1;
            var pageSize = request?.PageSize > 0 ? request.PageSize : 20;

            var query = _unitOfWork.Repository<Complaint>().GetAllQueryable()
                .AsNoTracking()
                .Where(c => c.Type == FeedbackType.Suggestion);

            if (IsOwnerOnly())
                query = query.Where(c => c.SuggestionStatus == SuggestionStatus.TransferredToOwner);
            else if (IsSupervisorOnly())
                query = query.Where(c => c.SuggestionStatus != SuggestionStatus.TransferredToOwner);

            var total = await query.CountAsync();
            var suggestions = await query
                .OrderByDescending(c => c.created_at)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ComplaintDto
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    Title = c.Title,
                    Content = c.content,
                    CreatedAt = c.created_at,
                    Type = c.Type,
                    Status = c.Status,
                    SuggestionStatus = c.SuggestionStatus,
                    Response = c.Response,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم استرجاع الاقتراحات بنجاح.", suggestions, page, pageSize, total);
        }

        public async Task<GeneralResponse> UpdateSuggestionStatusAsync(UpdateSuggestionStatusRequest request)
        {
            if (request?.SuggestionId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الاقتراح مطلوب.");

            if (!CanSupervisorManage())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحديث الحالة.");

            if (request.Status == SuggestionStatus.TransferredToOwner)
                return GeneralResponse.BadRequest("استخدم عملية تحويل للمالك بدلاً من تعديل الحالة مباشرة.");

            var suggestion = await _unitOfWork.Repository<Complaint>().GetByIdAsync(request.SuggestionId);
            if (suggestion == null || suggestion.Type != FeedbackType.Suggestion)
                return GeneralResponse.NotFound("الاقتراح غير موجود.");

            if (IsSupervisorOnly() && IsTransferredToOwner(suggestion))
                return GeneralResponse.Unauthorized("تم تحويل هذا الاقتراح إلى المالك.");

            suggestion.SuggestionStatus = request.Status;
            suggestion.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم تحديث حالة الاقتراح بنجاح.");
        }

        public async Task<GeneralResponse> TransferToOwnerAsync(Guid feedbackId)
        {
            if (feedbackId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الشكوى أو الاقتراح مطلوب.");

            if (!CanSupervisorManage())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحويل السجل إلى المالك.");

            var feedback = await _unitOfWork.Repository<Complaint>().GetByIdAsync(feedbackId);
            if (feedback == null)
                return GeneralResponse.NotFound("الشكوى أو الاقتراح غير موجود.");

            if (IsTransferredToOwner(feedback))
                return GeneralResponse.BadRequest("تم تحويل هذا السجل إلى المالك مسبقاً.");

            if (feedback.Type == FeedbackType.Complaint)
                feedback.Status = ComplaintStatus.TransferredToOwner;
            else
                feedback.SuggestionStatus = SuggestionStatus.TransferredToOwner;

            feedback.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم تحويل السجل إلى المالك بنجاح.");
        }

        public async Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId)
        {
            if (complaintId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الشكوى مطلوب.");

            if (!CanOwnerDelete())
                return GeneralResponse.Unauthorized("الحذف متاح للمالك أو المدير فقط.");

            var feedback = await _unitOfWork.Repository<Complaint>().GetByIdAsync(complaintId);
            if (feedback == null)
                return GeneralResponse.NotFound("الشكوى أو الاقتراح غير موجود.");

            if (IsOwnerOnly() && !IsTransferredToOwner(feedback))
                return GeneralResponse.Unauthorized("لا يمكن للمالك حذف سجل لم يتم تحويله إليه.");

            await _unitOfWork.Repository<Complaint>().DeleteAsync(feedback);
            await _unitOfWork.CompleteAsync();
            return GeneralResponse.Ok("تم حذف الشكوى/الاقتراح بنجاح.");
        }

        private bool IsAdmin() => _currentUserService.IsInRole("Admin");
        private bool IsOwner() => _currentUserService.IsInRole("Owner");
        private bool IsSupervisor() => _currentUserService.IsInRole("Supervisor");
        private bool IsOwnerOnly() => IsOwner() && !IsAdmin();
        private bool IsSupervisorOnly() => IsSupervisor() && !IsAdmin() && !IsOwner();
        private bool CanSupervisorManage() => IsAdmin() || IsSupervisor();
        private bool CanOwnerDelete() => IsAdmin() || IsOwner();
        private bool CanReadManagementFeedback() => IsAdmin() || IsOwner() || IsSupervisor();

        private static bool IsTransferredToOwner(Complaint feedback)
            => feedback.Type == FeedbackType.Complaint
                ? feedback.Status == ComplaintStatus.TransferredToOwner
                : feedback.SuggestionStatus == SuggestionStatus.TransferredToOwner;
    }
}
