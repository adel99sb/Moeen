using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Shared.Constants;

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
                var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("UserIdentifier")?.Value;
                return userIdClaim != null ? Guid.Parse(userIdClaim) : (Guid?)null;
            }
        }
        public bool? IsActived
        {
            get
            {
                var userActiveClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("IsActive")?.Value;
                bool state = bool.Parse(userActiveClaim);
                return userActiveClaim != null ? state : null;
            }
        }
        public bool? IsAdmin
        {
            get
            {
                var user = _httpContextAccessor.HttpContext?.User;

                if (user == null)
                    return null;

                return user.IsInRole(Roles.Owner.ToString());
            }
        }

        public string CurrentUserName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.Identity?.Name;
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