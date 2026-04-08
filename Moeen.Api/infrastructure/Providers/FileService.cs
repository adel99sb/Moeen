using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.infrastructure.Configurations;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Providers
{
    public class FileService : IFileService
    {
        private readonly string _rootPath;               // المسار الجذري لتخزين الملفات (مثل C:\Project\Uploads)
        private readonly ILogger<FileService> _logger;   // لتسجيل الأخطاء
        private readonly List<string> _allowedExtensions = new() { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".docx" }; // الامتدادات المسموحة
        private const long MaxFileSize = 10 * 1024 * 1024; // الحد الأقصى للحجم: 10 ميغابايت

        public FileService(
            IWebHostEnvironment environment,
            IOptions<FileStorageSettings> fileStorageSettings,
            ILogger<FileService> logger)
        {
            // قراءة المجلد الجذري من الإعدادات أو استخدام "Uploads" كقيمة افتراضية
            var configuredRoot = fileStorageSettings.Value.RootPath?.Trim() ?? "Uploads";
            _rootPath = Path.Combine(environment.ContentRootPath, configuredRoot);
            _logger = logger;
        }

        /// <summary>
        /// حفظ ملف مرفوع على الخادم
        /// </summary>
        /// <param name="file">الملف القادم من العميل</param>
        /// <param name="folder">المجلد الفرعي (مثل "khotbas" أو "students")</param>
        /// <returns>المسار النسبي للملف المحفوظ (مثل "khotbas/abc123.pdf")</returns>
        public async Task<string> SaveFileAsync(IFormFile file, string folder, CancellationToken cancellationToken = default)
        {
            // 1. التحقق من وجود الملف
            if (file is null || file.Length == 0)
                throw new ArgumentException("الملف مطلوب.", nameof(file));

            // 2. التحقق من حجم الملف (ألا يتجاوز 10 ميغابايت)
            if (file.Length > MaxFileSize)
                throw new ArgumentException($"حجم الملف يتجاوز الحد المسموح ({MaxFileSize / (1024 * 1024)} MB).");

            // 3. استخراج امتداد الملف وتحويله إلى أحرف صغيرة
            var extension = Path.GetExtension(file.FileName).ToLower();

            // 4. التحقق من أن الامتداد مسموح به
            if (!_allowedExtensions.Contains(extension))
                throw new ArgumentException($"نوع الملف {extension} غير مسموح. الامتدادات المسموحة: {string.Join(", ", _allowedExtensions)}");

            // 5. تطبيع اسم المجلد (إزالة المسافات والفواصل غير المتجانسة)
            folder = (folder ?? string.Empty).Trim().Replace("\\", "/").Trim('/');

            // 6. إنشاء اسم ملف فريد باستخدام GUID
            var newFileName = $"{Guid.NewGuid():N}{extension}";

            // 7. بناء المسار النسبي (سيُخزَّن في قاعدة البيانات)
            var relativePath = string.IsNullOrWhiteSpace(folder) ? newFileName : $"{folder}/{newFileName}";

            // 8. إنشاء المجلد الكامل على القرص (إذا لم يكن موجوداً)
            var fullDirectory = Path.Combine(_rootPath, folder);
            Directory.CreateDirectory(fullDirectory);

            // 9. المسار الكامل للملف الجديد
            var fullPath = Path.Combine(fullDirectory, newFileName);

            try
            {
                // 10. فتح تيار الكتابة ونسخ محتوى الملف إليه
                await using var stream = new FileStream(fullPath, FileMode.Create);
                await file.CopyToAsync(stream, cancellationToken);

                // 11. إرجاع المسار النسبي لحفظه في قاعدة البيانات
                return relativePath;
            }
            catch (Exception ex)
            {
                // 12. تسجيل الخطأ وإعادة رميه مع رسالة مفهومة
                _logger.LogError(ex, "فشل حفظ الملف {RelativePath}", relativePath);
                throw new IOException("تعذر حفظ الملف. راجع السجلات.", ex);
            }
        }

        /// <summary>
        /// حذف ملف من الخادم باستخدام مساره النسبي
        /// </summary>
        /// <returns>true إذا تم الحذف، false إذا لم يوجد الملف أو حدث خطأ</returns>
        public Task<bool> DeleteFileAsync(string relativePath)
        {
            // 1. التحقق من صحة المسار
            if (string.IsNullOrWhiteSpace(relativePath))
                return Task.FromResult(false);

            // 2. تحويل فواصل المسار إلى ما يناسب نظام التشغيل (مثلاً / → \ في ويندوز)
            relativePath = relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());

            // 3. بناء المسار الكامل المطلق
            var fullPath = Path.GetFullPath(Path.Combine(_rootPath, relativePath));
            var rootFullPath = Path.GetFullPath(_rootPath);

            // 4. منع هجوم اختراق المسار (Path Traversal)
            if (!fullPath.StartsWith(rootFullPath, StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            // 5. التحقق من وجود الملف فعلاً
            if (!File.Exists(fullPath))
                return Task.FromResult(false);

            try
            {
                // 6. حذف الملف
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                // 7. تسجيل الخطأ وإرجاع false (لا نعيد رمي الاستثناء لأن الحذف ليس حرجاً)
                _logger.LogError(ex, "فشل حذف الملف {RelativePath}", relativePath);
                return Task.FromResult(false);
            }
        }
    }
}