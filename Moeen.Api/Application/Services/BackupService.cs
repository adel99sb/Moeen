using Microsoft.AspNetCore.Hosting;
using Microsoft.Data.SqlClient;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Configurations;
using Moeen.Shared.Requests.Backup;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Backup;

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
                return GeneralResponse.InternalError("تعذر تحديد اسم قاعدة البيانات من إعدادات الاتصال.");

            Directory.CreateDirectory(_backupDirectory);

            var fileName = $"{databaseName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";
            var backupPath = Path.Combine(_backupDirectory, fileName);

            try
            {
                await using var connection = new SqlConnection(BuildMasterConnectionString());
                await connection.OpenAsync();

                try
                {
                    await ExecuteBackupAsync(connection, databaseName, backupPath, useCompression: true);
                }
                catch (SqlException)
                {
                    await ExecuteBackupAsync(connection, databaseName, backupPath, useCompression: false);
                }

                var fileInfo = new FileInfo(backupPath);
                return GeneralResponse.Ok("تم إنشاء النسخة الاحتياطية بنجاح.", new BackupInfoDto
                {
                    FileName = fileName,
                    SizeBytes = fileInfo.Exists ? fileInfo.Length : 0,
                    CreatedAtUtc = fileInfo.Exists ? fileInfo.LastWriteTimeUtc : DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"فشل إنشاء النسخة الاحتياطية: {ex.Message}");
            }
        }

        public Task<GeneralResponse> GetLastBackupInfoAsync()
        {
            if (!Directory.Exists(_backupDirectory))
                return Task.FromResult(GeneralResponse.NotFound("لا توجد نسخة احتياطية بعد."));

            var lastFile = new DirectoryInfo(_backupDirectory)
                .GetFiles("*.bak")
                .OrderByDescending(f => f.LastWriteTimeUtc)
                .FirstOrDefault();

            if (lastFile == null)
                return Task.FromResult(GeneralResponse.NotFound("لا توجد نسخة احتياطية بعد."));

            return Task.FromResult(GeneralResponse.Ok("تم جلب آخر نسخة احتياطية بنجاح.", new BackupInfoDto
            {
                FileName = lastFile.Name,
                SizeBytes = lastFile.Length,
                CreatedAtUtc = lastFile.LastWriteTimeUtc
            }));
        }

        public async Task<GeneralResponse> RestoreBackupAsync(RestoreBackupRequest request)
        {
            if (request?.Content == null || request.Content.Length == 0)
                return GeneralResponse.BadRequest("ملف النسخة الاحتياطية مطلوب.");

            var databaseName = GetDatabaseName();
            if (string.IsNullOrWhiteSpace(databaseName))
                return GeneralResponse.InternalError("تعذر تحديد اسم قاعدة البيانات من إعدادات الاتصال.");

            Directory.CreateDirectory(_backupDirectory);

            var safeFileName = Path.GetFileName(request.FileName);
            if (string.IsNullOrWhiteSpace(safeFileName))
                safeFileName = $"restore_{DateTime.UtcNow:yyyyMMdd_HHmmss}.bak";

            var backupPath = Path.Combine(_backupDirectory, safeFileName);
            await File.WriteAllBytesAsync(backupPath, request.Content);

            await using var connection = new SqlConnection(BuildMasterConnectionString());
            await connection.OpenAsync();

            try
            {
                await ExecuteNonQueryAsync(connection, $"ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;");

                await using var restore = new SqlCommand($"RESTORE DATABASE [{databaseName}] FROM DISK = @path WITH REPLACE;", connection)
                {
                    CommandTimeout = 300
                };
                restore.Parameters.AddWithValue("@path", backupPath);
                await restore.ExecuteNonQueryAsync();

                return GeneralResponse.Ok("تمت استعادة النسخة الاحتياطية بنجاح.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"فشل استعادة النسخة الاحتياطية: {ex.Message}");
            }
            finally
            {
                try
                {
                    await ExecuteNonQueryAsync(connection, $"ALTER DATABASE [{databaseName}] SET MULTI_USER;");
                }
                catch
                {
                    // Best effort: do not hide the original restore result.
                }
            }
        }

        private string GetDatabaseName()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);
            return builder.InitialCatalog;
        }

        private string BuildMasterConnectionString()
        {
            var builder = new SqlConnectionStringBuilder(_connectionString)
            {
                InitialCatalog = "master"
            };

            return builder.ConnectionString;
        }

        private static async Task ExecuteBackupAsync(SqlConnection connection, string databaseName, string backupPath, bool useCompression)
        {
            var compressionClause = useCompression ? ", COMPRESSION" : string.Empty;
            var commandText = $"BACKUP DATABASE [{databaseName}] TO DISK = @path WITH INIT{compressionClause};";

            await using var command = new SqlCommand(commandText, connection)
            {
                CommandTimeout = 300
            };
            command.Parameters.AddWithValue("@path", backupPath);
            await command.ExecuteNonQueryAsync();
        }

        private static async Task ExecuteNonQueryAsync(SqlConnection connection, string commandText)
        {
            await using var command = new SqlCommand(commandText, connection)
            {
                CommandTimeout = 300
            };

            await command.ExecuteNonQueryAsync();
        }
    }
}
