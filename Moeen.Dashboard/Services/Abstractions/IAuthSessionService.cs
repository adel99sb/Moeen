namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IAuthSessionService
    {
        string? AccessToken { get; }
        string? Role { get; }
        bool IsEmailConfirmed { get; }
        bool IsAuthenticated { get; }
        Task InitializeAsync();
        Task SetTokenAsync(string token);
        Task ClearAsync();
    }
}
