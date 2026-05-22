using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Configurations;
using Moeen.Shared.Requests.Backup;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Backup;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class BackupService : IBackupService
    {
        private readonly string _connectionString;
        private readonly string _backupDirectory;

        public BackupService(IWebHostEnvironment environment)
        {
            _connectionString = AppSettings.Instance.ConnectionString;
            _backupDirectory = Path.Combine(environment.ContentRootPath, "Backups");
        }

        public async Task<GeneralResponse> CreateBackupAsync()
        {
            var databaseName = GetDatabaseName();
            if (string.IsNullOrWhiteSpace(databaseName))
                return GeneralResponse.InternalError("«”„ ﬁ«⁄œ… «·»Ì«‰«  €Ì— „⁄—›.");

            Directory.CreateDirectory(_backupDirectory);

            var fileName = $"{databaseName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            var backupPath = Path.Combine(_backupDirectory, fileName);

            try
            {
                await using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                var commandText = $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH INIT, COMPRESSION;";
                await using var command = new SqlCommand(commandText, connection);
                command.Parameters.AddWithValue("@path", backupPath);

                await command.ExecuteNonQueryAsync();

                return GeneralResponse.Ok(" „ ≈‰‘«¡ «·‰”Œ… «·«Õ Ì«ÿÌ… »‰Ã«Õ.", new BackupInfoDto
                {
                    FileName = fileName,
                    SizeBytes = new FileInfo(backupPath).Length,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }
            catch
            {
                return GeneralResponse.InternalError("›‘· ≈‰‘«¡ «·‰”Œ… «·«Õ Ì«ÿÌ….");
            }
        }

        public Task<GeneralResponse> GetLastBackupInfoAsync()
        {
            if (!Directory.Exists(_backupDirectory))
                return Task.FromResult(GeneralResponse.NotFound("·«  ÊÃœ ‰”Œ «Õ Ì«ÿÌ… »⁄œ."));

            var lastFile = new DirectoryInfo(_backupDirectory)
                .GetFiles("*.bak")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();

            if (lastFile == null)
                return Task.FromResult(GeneralResponse.NotFound("·«  ÊÃœ ‰”Œ «Õ Ì«ÿÌ… »⁄œ."));

            return Task.FromResult(GeneralResponse.Ok(" „ Ã·» „⁄·Ê„«  ¬Œ— ‰”Œ… «Õ Ì«ÿÌ….", new BackupInfoDto
            {
                FileName = lastFile.Name,
                SizeBytes = lastFile.Length,
                CreatedAtUtc = lastFile.LastWriteTimeUtc
            }));
        }

        public async Task<GeneralResponse> RestoreBackupAsync(RestoreBackupRequest request)
        {
            if (request?.Content == null || request.Content.Length == 0)
                return GeneralResponse.BadRequest("„·› «·‰”Œ… «·«Õ Ì«ÿÌ… „ÿ·Ê».");

            var databaseName = GetDatabaseName();
            if (string.IsNullOrWhiteSpace(databaseName))
                return GeneralResponse.InternalError("«”„ ﬁ«⁄œ… «·»Ì«‰«  €Ì— „⁄—›.");

            Directory.CreateDirectory(_backupDirectory);

            var safeFileName = Path.GetFileName(request.FileName);
            if (string.IsNullOrWhiteSpace(safeFileName))
                safeFileName = $"restore_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";

            var backupPath = Path.Combine(_backupDirectory, safeFileName);
            await File.WriteAllBytesAsync(backupPath, request.Content);

            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            await using var connection = new SqlConnection(builder.ConnectionString);
            await connection.OpenAsync();

            try
            {
                await ExecuteNonQueryAsync(connection, $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");
                await using var restore = new SqlCommand($"RESTORE DATABASE [{databaseName}] FROM DISK = @path WITH REPLACE;", connection);
                restore.Parameters.AddWithValue("@path", backupPath);
                await restore.ExecuteNonQueryAsync();

                return GeneralResponse.Ok(" „  «” ⁄«œ… «·‰”Œ… «·«Õ Ì«ÿÌ… »‰Ã«Õ.");
            }
            catch
            {
                return GeneralResponse.InternalError("›‘·  ⁄„·Ì… «·«” ⁄«œ….");
            }
            finally
            {
                try
                {
                    await ExecuteNonQueryAsync(connection, $"ALTER DATABASE [{databaseName}] SET MULTI_USER;");
                }
                catch
                {
                    //  Ã«Â· √Ì √Œÿ«¡ √À‰«¡ ≈⁄«œ… MULTI_USER
                }
            }
        }

        private string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            return builder.InitialCatalog;
        }

        private static Task ExecuteNonQueryAsync(SqlConnection connection, string commandText)
        {
            var command = new SqlCommand(commandText, connection);
            return command.ExecuteNonQueryAsync();
        }
    }
}