namespace Moeen.Dashboard.Services.Abstractions
{
    public interface ITokenService
    {
        Task Save(string token);
        Task<string> Get();
        Task Clear();
    }
}
