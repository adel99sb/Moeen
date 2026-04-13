using Moeen.Api.Core.Constants;

namespace Moeen.Api.Core.Contracts.infrastructure.Providers
{
    public interface IFileService
    {
        Task<string> UploadFileAsync(FilePathType fileType, Guid ownerId, string fileName, byte[] fileData);
        Task<bool> DeleteFileAsync(string filePath);
        Task<string> GetFileUrlAsync(string filePath);
    }
}
