using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Complaint;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Complaint;

namespace Moeen.Api.Application.Services
{
    public class ComplaintService : IComplaintService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public ComplaintService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Complaint CRUD Operations

        public async Task<GeneralResponse> CreateComplaintAsync(CreateComplaintRequest createComplaintRequest)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(createComplaintRequest.UserId.ToString());
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");

                var complaint = new Complaint
                {
                    Id = Guid.NewGuid(),
                    UserId = createComplaintRequest.UserId,
                    Title = createComplaintRequest.Title,
                    Content = createComplaintRequest.Content,
                    Status = ComplaintStatus.Pending,
                    CreatedAt = DateTime.Now
                };

                await _unitOfWork.Repository<Complaint>().AddAsync(complaint);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Complaint submitted successfully.", new { complaint.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to submit complaint: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateComplaintStatusAsync(Guid complaintId, UpdateComplaintStatusRequest updateStatusRequest)
        {
            try
            {
                var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(complaintId);
                if (complaint == null)
                    return GeneralResponse.NotFound("Complaint not found.");

                complaint.Status = updateStatusRequest.Status;

                await _unitOfWork.Repository<Complaint>().UpdateAsync(complaint);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok($"Complaint status updated to {updateStatusRequest.Status} successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update complaint status: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteComplaintAsync(Guid complaintId)
        {
            try
            {
                var complaint = await _unitOfWork.Repository<Complaint>().GetByIdAsync(complaintId);
                if (complaint == null)
                    return GeneralResponse.NotFound("Complaint not found.");

                await _unitOfWork.Repository<Complaint>().DeleteAsync(complaint);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Complaint deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete complaint: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetComplaintByIdAsync(Guid complaintId)
        {
            try
            {
                var spec = Spec.For<Complaint>(c => c.Id == complaintId);
                spec.AddInclude(c => c.User);

                var complaint = (await _unitOfWork.Repository<Complaint>().GetAllAsync(spec)).FirstOrDefault();
                if (complaint == null)
                    return GeneralResponse.NotFound("Complaint not found.");

                var response = new ComplaintResponse
                {
                    Id = complaint.Id,
                    UserId = complaint.UserId,
                    UserFullName = complaint.User?.FullName,
                    UserEmail = complaint.User?.Email,
                    Title = complaint.Title,
                    Content = complaint.Content,
                    Status = complaint.Status,
                    CreatedAt = complaint.CreatedAt
                };

                return GeneralResponse.Ok("Complaint retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve complaint: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAllComplaintsAsync()
        {
            try
            {
                var spec = Spec.For<Complaint>(c => true);
                spec.AddInclude(c => c.User);

                var complaints = await _unitOfWork.Repository<Complaint>().GetAllAsync(spec);

                var response = complaints.Select(c => new ComplaintResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserFullName = c.User?.FullName,
                    UserEmail = c.User?.Email,
                    Title = c.Title,
                    Content = c.Content,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                }).ToList();

                return GeneralResponse.Ok("Complaints retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve complaints: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        public async Task<GeneralResponse> GetComplaintsByUserIdAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");

                var spec = Spec.For<Complaint>(c => c.UserId == userId);
                spec.AddInclude(c => c.User);

                var complaints = await _unitOfWork.Repository<Complaint>().GetAllAsync(spec);

                var response = complaints.Select(c => new ComplaintResponse
                {
                    Id = c.Id,
                    UserId = c.UserId,
                    UserFullName = c.User?.FullName,
                    UserEmail = c.User?.Email,
                    Title = c.Title,
                    Content = c.Content,
                    Status = c.Status,
                    CreatedAt = c.CreatedAt
                }).ToList();

                return GeneralResponse.Ok($"Complaints for user {user.FullName} retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve complaints for this user: {ex.Message}");
            }
        }

        #endregion
    }
}