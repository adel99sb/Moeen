using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.User;

namespace Moeen.Api.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly IVerificationService _verificationService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;

        public UserService(UserManager<User> userManager,
            IJwtService jwtService,
            IVerificationService verificationService,
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _verificationService = verificationService;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }
        public async Task<GeneralResponse> GetAllUsersAsync()
        {
            try
            {
                var usersFromDb = await _userManager.Users.ToListAsync();

                var users = await Task.WhenAll(usersFromDb.Select(async user => new UserResponse
                {
                    Id = user.Id.ToString(),
                    FullName = user.FullName,
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    ProfileImageUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) ? await _fileService.GetFileUrlAsync(user.ProfileImageUrl) ?? "" : "",
                    EmailConfirmed = user.EmailConfirmed,
                    UserRole = user.UserRole
                }));

                return GeneralResponse.Ok("Users retrieved successfully.", users.ToList());
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve users: {ex.Message}");
            }
        }
        public async Task<GeneralResponse> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");
            try
            {

                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                    await _fileService.DeleteFileAsync(user.ProfileImageUrl);
                await _userManager.DeleteAsync(user);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"delete user failed: {ex.Message}");
            }
            return GeneralResponse.Ok("User deleted successfully.");
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.email);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.password);
            if (!result)
                return GeneralResponse.BadRequest("Invalid password.");
            LoginResponse data = null;
            try
            {
                var token = await _jwtService.GenerateJwtToken(user, _userManager);

                await _unitOfWork.CompleteAsync();

                data = new LoginResponse() { AccessToken = token,};
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Login failed : {ex.Message}");
            }
            return GeneralResponse.Ok("Login successful.", data);
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.email) != null)
                return GeneralResponse.BadRequest("Email is already in use.");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = registerRequest.email,
                PhoneNumber = registerRequest.Phone,
                EmailConfirmed = false,
                FullName = registerRequest.fullName,
                UserName = Guid.NewGuid().ToString(),
                UserRole = registerRequest.userType
            };
            if (registerRequest.Picture != null)
            {
                if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                {
                    var Deletedpath = user.ProfileImageUrl.Replace(_currentUserService.GetBaseUrl(""), "").Replace("\\\\", "\\");
                    await _fileService.DeleteFileAsync(Deletedpath);
                }
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(registerRequest.Picture.FileName)}";

                using var memoryStream = new MemoryStream();
                await registerRequest.Picture.CopyToAsync(memoryStream);
                var fileBytes = memoryStream.ToArray();

                var savedPath = await _fileService.UploadFileAsync(FilePathType.UserProfiles, user.Id, fileName, fileBytes);

                user.ProfileImageUrl = savedPath;
            }
            try
            {
                await _userManager.CreateAsync(user, registerRequest.password);
                await _userManager.AddToRoleAsync(user, registerRequest.userType.ToString());
                await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Email Verification","ar");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Registration failed: {ex.Message}");
            }
            return GeneralResponse.Ok("User created. Please check your email for verification code.");
        }

        public async Task<GeneralResponse> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest)
        {
            var user = await _userManager.FindByIdAsync(resetPasswordRequest.Uid.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");
            var internalToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            try
            {
                var res = await _userManager.ResetPasswordAsync(user,internalToken, resetPasswordRequest.NewPassword);
                if (!res.Succeeded)
                {
                    return GeneralResponse.BadRequest("invalid token");
                }
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Reset password failed: {ex.Message}");
            }
            return GeneralResponse.Ok("Password reset successfully.");
        }

        public async Task<GeneralResponse> SendVerifyEmailCodeAsync(SendVerifyEmailCodeRequest sendVerifyEmailCodeRequest)
        {
            var user = await _userManager.FindByEmailAsync(sendVerifyEmailCodeRequest.email);
            if (user == null)
                return GeneralResponse.NotFound("User not found ,Please register first.");
                try
                {
                    await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Email Verification","ar");
                }
                catch (Exception ex)
                {
                    return GeneralResponse.InternalError($"Send verification code failed: {ex.Message}");
                }
            return GeneralResponse.Ok("Please check your email for verification code.");
        }

        public async Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(verifyEmailRequest.email);
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");
                if (!await _verificationService.VerifyCodeAsync(user.Id, verifyEmailRequest.verificationCode))
                    return GeneralResponse.BadRequest("Invalid verification code.");
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Verify email failed: {ex.Message}");
            }

            return GeneralResponse.Ok("Email successfully verified.");
        }

        public async Task<GeneralResponse> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            return GeneralResponse.Ok("User retrieved successfully.", new UserResponse
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.PhoneNumber,
                ProfileImageUrl = !string.IsNullOrEmpty(user.ProfileImageUrl) ? await _fileService.GetFileUrlAsync(user.ProfileImageUrl) ?? "" : "",
                EmailConfirmed = user.EmailConfirmed,
                UserRole = user.UserRole
            });
        }
    }
}
