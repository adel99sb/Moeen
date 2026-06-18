using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;
using System.Security.Claims;

namespace Moeen.Api.infrastructure.Providers
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid? CurrentUserId
        {
            get
            {
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserIdentifier")?.Value
                    ?? _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                return Guid.TryParse(userIdClaim, out var parsedUserId) ? parsedUserId : null;
            }
        }
        public bool? IsActived
        {
            get
            {
                var userActiveClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("IsActive")?.Value;
                return bool.TryParse(userActiveClaim, out var state) ? state : null;
            }
        }
        public bool? IsAdmin
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user == null)
                    return null;

                return user.IsInRole(Roles.Owner.ToString())
                    || user.IsInRole(Roles.Admin.ToString());
            }
        }

        public bool IsInRole(string roleName)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return false;

            return _httpContextAccessor.HttpContext?.User?.IsInRole(roleName) == true;
        }

        public string CurrentUserName
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;
                return user?.FindFirst("FullName")?.Value
                    ?? user?.FindFirst(ClaimTypes.Name)?.Value
                    ?? user?.Identity?.Name
                    ?? string.Empty;
            }
        }
        public string GetBaseUrl(string relativePath)
        {
            var request = _httpContextAccessor.HttpContext?.Request;
            if (request == null) return relativePath;

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{relativePath}";
        }
    }
}
