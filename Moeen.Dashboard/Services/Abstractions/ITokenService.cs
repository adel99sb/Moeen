namespace Moeen.Dashboard.Services.Abstractions
{
    public sealed record DashboardAuthSession(
        bool IsAuthenticated,
        string? Token,
        IReadOnlyList<string> Roles,
        string? PrimaryRole,
        string HomeRoute);

    public interface ITokenService
    {
        Task Save(string token);
        Task<string?> Get();
        Task Clear();
        Task<DashboardAuthSession> GetSession();
    }
}
