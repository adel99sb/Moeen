using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

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

        public async Task<GeneralResponse> ChangeUserEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return GeneralResponse.BadRequest("Email is required.");

            var currentUserId = _currentUserService.CurrentUserId;
            if (currentUserId == null)
                return GeneralResponse.Unauthorized("User not authenticated.");

            try
            {
                var user = await _unitOfWork.Repository<User>().GetByIdAsync(currentUserId.Value);
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");

                user.Email = email;
                user.EmailConfirmed = false;

                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    var errors = string.Join("; ", updateResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"Update email failed: {errors}");
                }

                await _verificationService.SendVerificationCodeAsync(user.Id, email, "Email Verification", "ar");
                return GeneralResponse.Ok("Email updated. Verification code sent to the new email.");
            }
            catch
            {
                return GeneralResponse.InternalError("Change email failed.");
            }
        }

        public async Task<GeneralResponse> GetAllUsersAsync(PaginationRequest paginationRequest, string? keyword)
        {
            var spec = Spec.For<User>(null);

            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Replace(" ", "").Replace("+", "").Replace("%20", "");
                spec.AddCriteria(u => u.name.Contains(keyword)
                            || u.Email.Contains(keyword)
                            || u.PhoneNumber.Contains(keyword));
            }

            var Countspec = Spec.For(spec.Criteria);

            int skip = (paginationRequest.Page - 1) * paginationRequest.PageSize;
            spec.ApplyPaging(skip, paginationRequest.PageSize);
            spec.ApplyOrderByDescending(u => u.created_at);

            var users = (await _unitOfWork.Repository<User>().GetAllAsync(spec)).ToList();
            var userCount = (await _unitOfWork.Repository<User>().GetAllAsync(Countspec)).Count();

            if (users.Count == 0)
                return GeneralResponse.Ok("Get 0 users successfully", new List<UserDto>(),
                    page: paginationRequest.Page,
                    pageSize: paginationRequest.PageSize,
                    totalCount: 0);

            // Batch GetRolesAsync and GetFileUrlAsync to avoid N+1 sequential calls
            var roleTasks = users.Select(u => _userManager.GetRolesAsync(u)).ToList();
            var fileTasks = users.Select(u => _fileService.GetFileUrlAsync(u.profile_imageUrl)).ToList();

            await Task.WhenAll(roleTasks.Cast<Task>());
            await Task.WhenAll(fileTasks);

            var rolesResults = roleTasks.Select(t => t.Result).ToList();
            var filesResults = fileTasks.Select(t => t.Result).ToList();

            var data = new List<UserDto>();
            for (int i = 0; i < users.Count; i++)
            {
                var userData = users[i];
                var userRoles = rolesResults[i] ?? new List<string>();

                if (userRoles.Contains(Roles.Owner.ToString()))
                {
                    userCount--; // preserve original behavior: decrement total when skipping owner
                    continue;
                }

                var user = new UserDto()
                {
                    Id = userData.Id,
                    CreatedAt = userData.created_at,
                    Email = userData.Email,
                    FontSize = userData.font_size,
                    Gender = userData.gender,
                    JoinedAt = userData.JoinedAt,
                    Name = userData.name,
                    Phone = userData.PhoneNumber,
                };

                var imagePath = filesResults[i];
                user.ProfileImageUrl = imagePath != null ? imagePath : null;

                if (userRoles.Count > 0)
                    user.Roles.AddRange(userRoles);

                data.Add(user);
            }

            return GeneralResponse.Ok($"Get {data.Count} users successfully", data,
                page: paginationRequest.Page,
                pageSize: paginationRequest.PageSize,
                totalCount: userCount);
        }

        public async Task<GeneralResponse> GetUserByIdAsync(Guid userId)
        {
            var userData = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            if (userData == null)
                return GeneralResponse.NotFound("User not found !");

            var data = new UserDto()
            {
                Id = userData.Id,
                CreatedAt = userData.created_at,
                Email = userData.Email,
                FontSize = userData.font_size,
                Gender = userData.gender,
                JoinedAt = userData.JoinedAt,
                Name = userData.name,
                Phone = userData.PhoneNumber,
            };
            var imagePath = (await _fileService.GetFileUrlAsync(userData.profile_imageUrl));
            data.ProfileImageUrl = imagePath != null ? imagePath : null;
            var userRoles = await _userManager.GetRolesAsync(userData);
            if (userRoles.Count > 0)
                data.Roles.AddRange(userRoles);
            return GeneralResponse.Ok("Get user data successful.", data);
        }

        public async Task<GeneralResponse> LoginAsync(LoginRequest loginRequest)
        {
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            var result = await _userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!result)
                return GeneralResponse.BadRequest("Invalid password.");

            AuthResponse data = null;
            try
            {
                var token = await _jwtService.GenerateJwtToken(user, _userManager);
                data = new AuthResponse() { Token = token };
            }
            catch
            {
                return GeneralResponse.InternalError("Login failed.");
            }
            return GeneralResponse.Ok("Login successful.", data);
        }

        public async Task<GeneralResponse> RegisterAsync(RegisterRequest registerRequest)
        {
            if (await _userManager.FindByEmailAsync(registerRequest.Email) != null)
                return GeneralResponse.BadRequest("Email is already in use.");

            var user = new User
            {
                Email = registerRequest.Email,
                EmailConfirmed = false,
                name = registerRequest.Name,
                UserName = Guid.NewGuid().ToString(),
                gender = registerRequest.Gender,
                PhoneNumber = registerRequest.Phone,
                created_at = DateTime.UtcNow,
                JoinedAt = DateTime.UtcNow
            };
            try
            {
                var res = await _userManager.CreateAsync(user, registerRequest.Password);
                if (!res.Succeeded)
                {
                    var errors = string.Join("; ", res.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"Registration failed: {errors}");
                }

                await _userManager.AddToRoleAsync(user, Roles.Student.ToString());
                 await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Email Verification", "ar");
            }
            catch
            {
                return GeneralResponse.InternalError("Registration failed.");
            }
            return GeneralResponse.Ok("User created. Please check your email for verification code.");
        }

        public async Task<GeneralResponse> ResetPasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            if (changePasswordRequest == null)
                return GeneralResponse.BadRequest("Invalid request.");

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(changePasswordRequest.UserId);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            try
            {
                // Read Token from request (if ChangePasswordRequest was extended), fallback -> error
                string? token = null;
                var tokenProp = changePasswordRequest.GetType().GetProperty("Token", BindingFlags.Public | BindingFlags.Instance);
                if (tokenProp != null)
                    token = tokenProp.GetValue(changePasswordRequest) as string;

                if (string.IsNullOrWhiteSpace(token))
                    return GeneralResponse.BadRequest("Reset token is required.");

                var resetResult = await _userManager.ResetPasswordAsync(user, token, changePasswordRequest.NewPassword);
                if (!resetResult.Succeeded)
                {
                    var errors = string.Join("; ", resetResult.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"Reset password failed: {errors}");
                }
                return GeneralResponse.Ok("Password reset successfully.");
            }
            catch
            {
                return GeneralResponse.InternalError("Reset password failed.");
            }
        }

        public async Task<GeneralResponse> SendPasswordResetUrlAsync(SendPasswordResetUrlRequest sendPasswordResetUrlRequest)
        {
            string? email = null;
            if (sendPasswordResetUrlRequest != null)
            {
                var prop = sendPasswordResetUrlRequest.GetType().GetProperty("Email", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                    email = prop.GetValue(sendPasswordResetUrlRequest) as string;
            }

            if (string.IsNullOrWhiteSpace(email))
            {
                if (_currentUserService.CurrentUserId == null)
                    return GeneralResponse.BadRequest("Email is required.");
                var currentUser = await _unitOfWork.Repository<User>().GetByIdAsync(_currentUserService.CurrentUserId.Value);
                if (currentUser == null || string.IsNullOrWhiteSpace(currentUser.Email))
                    return GeneralResponse.BadRequest("Email is required.");
                email = currentUser.Email;
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return GeneralResponse.NotFound("User not found.");

            try
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var pageUrl = _currentUserService.GetBaseUrl("/reset-password");
                await _verificationService.SendRestpageUrlAsync(email, "Reset Password", token, pageUrl, "ar");
                return GeneralResponse.Ok("Password reset link sent to email.");
            }
            catch
            {
                return GeneralResponse.InternalError("Send password reset url failed.");
            }
        }

        public async Task<GeneralResponse> SendVerifyEmailCodeAsync(SendVerifyEmailCodeRequest sendVerifyEmailCodeRequest)
        {
            string? email = null;
            if (sendVerifyEmailCodeRequest != null)
            {
                var prop = sendVerifyEmailCodeRequest.GetType().GetProperty("Email", BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                    email = prop.GetValue(sendVerifyEmailCodeRequest) as string;
            }

            User? user = null;
            if (!string.IsNullOrWhiteSpace(email))
            {
                user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");
            }
            else
            {
                if (_currentUserService.CurrentUserId == null)
                    return GeneralResponse.Unauthorized("User not authenticated.");
                user = await _unitOfWork.Repository<User>().GetByIdAsync(_currentUserService.CurrentUserId.Value);
                if (user == null)
                    return GeneralResponse.NotFound("User not found.");
                email = user.Email;
            }

            try
            {
                await _verificationService.SendVerificationCodeAsync(user.Id, email, "Email Verification", "ar");
                return GeneralResponse.Ok("Verification code sent.");
            }
            catch
            {
                return GeneralResponse.InternalError("Send verification code failed.");
            }
        }

        public async Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest)
        {
            if (verifyEmailRequest == null)
                return GeneralResponse.BadRequest("Invalid request.");

            try
            {
                string? code = null;
                string? email = null;
                Guid? userId = null;

                var codeProp = verifyEmailRequest.GetType().GetProperty("Code", BindingFlags.Public | BindingFlags.Instance);
                if (codeProp != null) code = codeProp.GetValue(verifyEmailRequest) as string;

                var emailProp = verifyEmailRequest.GetType().GetProperty("Email", BindingFlags.Public | BindingFlags.Instance);
                if (emailProp != null) email = emailProp.GetValue(verifyEmailRequest) as string;

                var idProp = verifyEmailRequest.GetType().GetProperty("UserId", BindingFlags.Public | BindingFlags.Instance);
                if (idProp != null)
                {
                    var val = idProp.GetValue(verifyEmailRequest);
                    if (val is Guid g) userId = g;
                    else if (Guid.TryParse(val?.ToString(), out var parsed)) userId = parsed;
                }

                if (string.IsNullOrWhiteSpace(code))
                    return GeneralResponse.BadRequest("Verification code is required.");

                User? user = null;
                if (userId.HasValue)
                {
                    user = await _unitOfWork.Repository<User>().GetByIdAsync(userId.Value);
                    if (user == null) return GeneralResponse.NotFound("User not found.");
                }
                else if (!string.IsNullOrWhiteSpace(email))
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null) return GeneralResponse.NotFound("User not found.");
                }
                else
                {
                    if (_currentUserService.CurrentUserId == null)
                        return GeneralResponse.Unauthorized("User not authenticated.");
                    user = await _unitOfWork.Repository<User>().GetByIdAsync(_currentUserService.CurrentUserId.Value);
                    if (user == null) return GeneralResponse.NotFound("User not found.");
                }

                var verified = await _verificationService.VerifyCodeAsync(user.Id, code);
                if (!verified)
                    return GeneralResponse.BadRequest("Invalid or expired verification code.");

                user.EmailConfirmed = true;
                var upd = await _userManager.UpdateAsync(user);
                if (!upd.Succeeded)
                {
                    var errors = string.Join("; ", upd.Errors.Select(e => e.Description));
                    return GeneralResponse.BadRequest($"Verification update failed: {errors}");
                }

                return GeneralResponse.Ok("Email verified successfully.");
            }
            catch
            {
                return GeneralResponse.InternalError("Verify email failed.");
            }
        }
    }
}
