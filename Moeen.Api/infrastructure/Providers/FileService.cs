using Microsoft.Extensions.Options;
using Moeen.Api.Core.Constants;
using Moeen.Api.Core.Contracts.infrastructure.Providers;

namespace Moeen.Api.infrastructure.Providers
{
    public class FileService : IFileService
    {
        private readonly string _baseDirectory;
        private readonly ICurrentUserService _currentUserService;

        public FileService(ICurrentUserService currentUserService)
        {
            _baseDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");

            if (!Directory.Exists(_baseDirectory))
                Directory.CreateDirectory(_baseDirectory);
            _currentUserService = currentUserService;
        }

        public async Task<string> UploadFileAsync(FilePathType fileType, Guid ownerId, string fileName, byte[] fileData)
        {
            var relativeFolder = FilePathConstants.PathMappings[fileType];
            string targetFolder;
            string fullFolderPath;

            targetFolder = Path.Combine(relativeFolder, ownerId.ToString());

            fullFolderPath = Path.Combine(Directory.GetCurrentDirectory(), targetFolder);


            if (!Directory.Exists(fullFolderPath))
                Directory.CreateDirectory(fullFolderPath);

            var filePath = Path.Combine(fullFolderPath, fileName);

            await File.WriteAllBytesAsync(filePath, fileData);

            var relativePath = Path.Combine("/", targetFolder.Replace("\\", "/"), fileName.Replace("\\", "/"));
            return relativePath;
        }

        public Task<bool> DeleteFileAsync(string relativePath)
        {
            var fileFullPath = Path.Combine(Directory.GetCurrentDirectory(), relativePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(fileFullPath))
            {
                File.Delete(fileFullPath);
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public async Task<string> GetFileUrlAsync(string relativePath)
        {
            if (!string.IsNullOrEmpty(relativePath))
            {
                var imagePath = _currentUserService.GetBaseUrl(relativePath);
                return await Task.FromResult(imagePath);
            }
            return null;
        }
    }
}