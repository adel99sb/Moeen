using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Mosque;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mosque;

namespace Moeen.Api.Application.Services
{
    public class MosqueService : IMosqueService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public MosqueService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Mosque CRUD Operations

        public async Task<GeneralResponse> CreateMosqueAsync(CreateMosqueRequest createMosqueRequest)
        {
            try
            {
                var mosque = new Mosque
                {
                    Id = Guid.NewGuid(),
                    Name = createMosqueRequest.Name,
                    Address = createMosqueRequest.Address
                };

                await _unitOfWork.Repository<Mosque>().AddAsync(mosque);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Mosque created successfully.", new { mosque.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to create mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateMosqueAsync(Guid mosqueId, CreateMosqueRequest updateMosqueRequest)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                mosque.Name = updateMosqueRequest.Name;
                mosque.Address = updateMosqueRequest.Address;

                await _unitOfWork.Repository<Mosque>().UpdateAsync(mosque);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Mosque updated successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteMosqueAsync(Guid mosqueId)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                await _unitOfWork.Repository<Mosque>().DeleteAsync(mosque);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Mosque deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetMosqueByIdAsync(Guid mosqueId)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var mosqueResponse = new MosqueResponse
                {
                    Id = mosque.Id,
                    Name = mosque.Name,
                    Address = mosque.Address
                };

                return GeneralResponse.Ok("Mosque retrieved successfully.", mosqueResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAllMosquesAsync()
        {
            try
            {
                var mosques = await _unitOfWork.Repository<Mosque>().GetAllAsync();

                var mosquesResponse = mosques.Select(m => new MosqueResponse
                {
                    Id = m.Id,
                    Name = m.Name,
                    Address = m.Address
                }).ToList();

                return GeneralResponse.Ok("Mosques retrieved successfully.", mosquesResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve mosques: {ex.Message}");
            }
        }

        #endregion

        #region Mosque Users Management (Many-to-Many)

        public async Task<GeneralResponse> AddUserToMosqueAsync(AddUserToMosqueRequest addUserToMosqueRequest)
        {
            try
            {
                // 1. التحقق من وجود المسجد
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(addUserToMosqueRequest.MosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                // 2. التحقق من وجود المستخدم عبر الـ UserManager
                var user = await _userManager.FindByIdAsync(addUserToMosqueRequest.UserId.ToString());
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");

                // 3. التحقق مما إذا كان المستخدم مضافاً مسبقاً منعاً للتكرار الدوبليكيت
                var spec = Spec.For<MosqueUser>(mu =>
                    mu.MosqueId == addUserToMosqueRequest.MosqueId && mu.UserId == addUserToMosqueRequest.UserId);

                var existingLink = (await _unitOfWork.Repository<MosqueUser>().GetAllAsync(spec)).FirstOrDefault();

                if (existingLink != null)
                    return GeneralResponse.BadRequest("User is already assigned to this mosque.");

                // 4. عملية الربط
                var mosqueUser = new MosqueUser
                {
                    MosqueId = addUserToMosqueRequest.MosqueId,
                    UserId = addUserToMosqueRequest.UserId
                };

                await _unitOfWork.Repository<MosqueUser>().AddAsync(mosqueUser);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("User added to mosque successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to add user to mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> RemoveUserFromMosqueAsync(Guid mosqueId, Guid userId)
        {
            try
            {
                var spec = Spec.For<MosqueUser>(mu =>
                    mu.MosqueId == mosqueId && mu.UserId == userId);
                var mosqueUser = (await _unitOfWork.Repository<MosqueUser>().GetAllAsync(spec)).FirstOrDefault();

                if (mosqueUser == null)
                    return GeneralResponse.NotFound("The relation between this user and mosque does not exist.");

                await _unitOfWork.Repository<MosqueUser>().DeleteAsync(mosqueUser);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("User removed from mosque successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to remove user from mosque: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetMosqueUsersAsync(Guid mosqueId)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var spec = Spec.For<MosqueUser>(mu => mu.MosqueId == mosqueId);
                spec.AddInclude(m => m.User);

                var mosqueUsers = await _unitOfWork.Repository<MosqueUser>().GetAllAsync(spec);

                var usersResponse = mosqueUsers
                    .Select(mu => new UserResponse
                    {
                        Id = mu.User.Id.ToString(),
                        FullName = mu.User.FullName,
                        Email = mu.User.Email,
                        Phone = mu.User.PhoneNumber,
                        ProfileImageUrl = mu.User.ProfileImageUrl,
                        UserRole = mu.User.UserRole,
                        EmailConfirmed = mu.User.EmailConfirmed
                    })
                    .ToList();

                return GeneralResponse.Ok("Mosque users retrieved successfully.", usersResponse);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve mosque users: {ex.Message}");
            }
        }

        #endregion
    }
}