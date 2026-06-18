namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface ICurrentUserService
    {
        Guid? CurrentUserId { get; }
        string CurrentUserName { get; }
        bool? IsActived { get; }
        bool? IsAdmin { get; }
        bool IsInRole(string roleName);
        string GetBaseUrl(string relativePath);
    }
}
