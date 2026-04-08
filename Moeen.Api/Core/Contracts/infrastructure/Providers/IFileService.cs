using Microsoft.AspNetCore.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default);
        Task<bool> DeleteFileAsync(string relativePath);
    }
}
