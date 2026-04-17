using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Contracts;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Api.Shared.Requests;
using Moeen.Api.Shared.Requests.Identity;
using Moeen.Api.Shared.Responses;
using Moeen.Api.Shared.Responses.Identity;

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
        public Task<GeneralResponse> ChangeUserEmailAsync(string email)
        {
            throw new NotImplementedException();
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
            var users = await _unitOfWork.Repository<User>().GetAllAsync(spec);
            var userCount = (await _unitOfWork.Repository<User>().GetAllAsync(Countspec)).Count();
            var data = new List<UserDto>();
            foreach (var userData in users)
            {
                if (await _userManager.IsInRoleAsync(userData, Roles.Owner.ToString()))
                {
                    userCount--;
                    continue;
                }
                var user = new UserDto()
                {
                    Id = userData.Id,
                    CreatedAt = userData.created_at,
                    Email = userData.Email,
                    FontSize = userData.font_size,
                    Gender = userData.gender,
                    JoinedAt = userData.joinef_at,
                    Name = userData.name,
                    Phone = userData.PhoneNumber,
                };
                var imagePath = (await _fileService.GetFileUrlAsync(userData.profile_imageUrl));
                user.ProfileImageUrl = imagePath != null ? imagePath : null;
                var userRoles = await _userManager.GetRolesAsync(userData);
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
            var user = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            if (user == null)
                return GeneralResponse.NotFound("User not found !");
            var userData = await _unitOfWork.Repository<User>().GetByIdAsync(userId);
            var data = new UserDto()
            {
                Id = userData.Id,
                CreatedAt = userData.created_at,
                Email = userData.Email,
                FontSize = userData.font_size,
                Gender = userData.gender,
                JoinedAt = userData.joinef_at,
                Name = userData.name,
                Phone = userData.PhoneNumber,
            };
            var imagePath = (await _fileService.GetFileUrlAsync(userData.profile_imageUrl));
            data.ProfileImageUrl = imagePath != null ? imagePath : null;
            var userRoles = await _userManager.GetRolesAsync(user);
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
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Login failed : {ex.Message}");
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
                name= registerRequest.Name,
                UserName = Guid.NewGuid().ToString(),
                gender = registerRequest.Gender,
                PhoneNumber = registerRequest.Phone,                
            };
            try
            {
                var res = await _userManager.CreateAsync(user, registerRequest.Password);
                await _userManager.AddToRoleAsync(user, Roles.Student.ToString());
                await _verificationService.SendVerificationCodeAsync(user.Id, user.Email, "Email Verification", "ar");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Registration failed: {ex.Message}");
            }
            return GeneralResponse.Ok("User created. Please check your email for verification code.");
        }

        public Task<GeneralResponse> ResetPasswordAsync(ChangePasswordRequest changePasswordRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> SendPasswordResetUrlAsync(SendPasswordResetUrlRequest sendPasswordResetUrlRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> SendVerifyEmailCodeAsync(SendVerifyEmailCodeRequest sendVerifyEmailCodeRequest)
        {
            throw new NotImplementedException();
        }

        public Task<GeneralResponse> VerifyEmailAsync(VerifyEmailRequest verifyEmailRequest)
        {
            throw new NotImplementedException();
        }
    }
}
