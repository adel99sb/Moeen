using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Providers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<CurrentUserService> _logger;
        private User? _cachedUser; // لتجنب جلب المستخدم أكثر من مرة في نفس الطلب

        public CurrentUserService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            ILogger<CurrentUserService> logger)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        private ClaimsPrincipal? CurrentUser => _httpContextAccessor.HttpContext?.User;

        public Guid? UserId
        {
            get
            {
                var idValue = CurrentUser?.FindFirst("ID")?.Value
                              ?? CurrentUser?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(idValue, out var parsedId) ? parsedId : null;
            }
        }

        public string? UserName =>
            CurrentUser?.FindFirst("fulName")?.Value
            ?? CurrentUser?.Identity?.Name;

        public string? Email =>
            CurrentUser?.FindFirst(JwtRegisteredClaimNames.Email)?.Value
            ?? CurrentUser?.FindFirst(ClaimTypes.Email)?.Value;

        public bool IsAuthenticated => CurrentUser?.Identity?.IsAuthenticated == true;

        // خاصية الجامع الحالي من الـ Claim
        public Guid? CurrentMosqueId
        {
            get
            {
                var mosqueIdStr = CurrentUser?.FindFirst("MosqueId")?.Value;
                return Guid.TryParse(mosqueIdStr, out var id) ? id : null;
            }
        }

        public async Task<List<string>> GetUserRolesAsync()
        {
            if (!IsAuthenticated)
                return new List<string>();

            var user = await GetCurrentUserEntityAsync();
            if (user is null)
                return new List<string>();

            var roles = await _userManager.GetRolesAsync(user);
            return roles.ToList();
        }

        public Task<List<Claim>> GetUserClaimsAsync()
        {
            var claims = CurrentUser?.Claims?.ToList() ?? new List<Claim>();
            return Task.FromResult(claims);
        }

        public async Task<bool> HasPermissionAsync(string permission)
        {
            if (string.IsNullOrWhiteSpace(permission) || !IsAuthenticated)
                return false;

            var claims = await GetUserClaimsAsync();
            return claims.Any(c =>
                (c.Type.Equals("permission", StringComparison.OrdinalIgnoreCase) ||
                 c.Type.Equals("permissions", StringComparison.OrdinalIgnoreCase)) &&
                c.Value.Equals(permission, StringComparison.OrdinalIgnoreCase));
        }

        public async Task<User?> GetCurrentUserEntityAsync()
        {
            if (_cachedUser != null)
                return _cachedUser;

            if (UserId is null)
                return null;

            try
            {
                _cachedUser = await _userManager.FindByIdAsync(UserId.Value.ToString());
                return _cachedUser;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب المستخدم {UserId}", UserId);
                return null;
            }
        }
    }
}