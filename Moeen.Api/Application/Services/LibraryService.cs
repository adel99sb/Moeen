using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Library;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;

        public LibraryService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }

        public async Task<GeneralResponse> AddBookAsync(AddBookRequest request)
        {
            if (request?.BookData == null)
                return GeneralResponse.BadRequest("بيانات الكتاب مطلوبة.");

            if (string.IsNullOrWhiteSpace(request.BookData.Title))
                return GeneralResponse.BadRequest("اسم الكتاب مطلوب.");

            if (string.IsNullOrWhiteSpace(request.BookData.Description))
                return GeneralResponse.BadRequest("وصف الكتاب مطلوب.");

            if (!await CanManageLibraryAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بإضافة الكتب.");

            var placement = await ResolveLibraryPlacementAsync();
            if (placement is null)
                return GeneralResponse.BadRequest("لا توجد بيانات مسجد/درس سبت مرتبطة لحفظ الكتاب.");

            var fileUrl = request.BookData.CoverImageUrl?.Trim();

            if (request.BookFile != null && request.BookFile.Length > 0)
            {
                var safeName = SanitizeFileName(request.BookData.Title);
                var fileName = $"{safeName}_{Guid.NewGuid():N}.pdf";
                fileUrl = await _fileService.UploadFileAsync(FilePathType.LibraryBooks, placement.Value.MosqueId, fileName, request.BookFile);
            }

            if (string.IsNullOrWhiteSpace(fileUrl))
                return GeneralResponse.BadRequest("ملف الكتاب أو رابط التحميل مطلوب.");

            var uploaderName = await ResolveCurrentUserDisplayNameAsync();

            var book = new PdfFile
            {
                Id = Guid.NewGuid(),
                MosqueId = placement.Value.MosqueId,
                SaturdayLessonId = placement.Value.SaturdayLessonId,
                FileUrl = fileUrl,
                title = request.BookData.Title.Trim(),
                description = request.BookData.Description.Trim(),
                uploaded_by = uploaderName,
                created_at = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PdfFile>().AddAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تمت إضافة الكتاب بنجاح.", await MapBookAsync(book));
        }

        public async Task<GeneralResponse> UpdateBookInfoAsync(UpdateBookRequest request)
        {
            if (request?.BookData == null || request.Id == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الكتاب وبياناته مطلوبة.");

            if (!await CanManageLibraryAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بتحديث الكتب.");

            var book = await _unitOfWork.Repository<PdfFile>().GetByIdAsync(request.Id);
            if (book == null)
                return GeneralResponse.NotFound("الكتاب غير موجود.");

            if (!await CanAccessBookAsync(book))
                return GeneralResponse.Unauthorized("لا يمكنك الوصول إلى كتاب تابع لمسجد آخر.");

            if (!string.IsNullOrWhiteSpace(request.BookData.Title))
                book.title = request.BookData.Title.Trim();

            if (!string.IsNullOrWhiteSpace(request.BookData.Description))
                book.description = request.BookData.Description.Trim();

            if (!string.IsNullOrWhiteSpace(request.BookData.CoverImageUrl))
                book.FileUrl = request.BookData.CoverImageUrl.Trim();

            await _unitOfWork.Repository<PdfFile>().UpdateAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تحديث بيانات الكتاب بنجاح.", await MapBookAsync(book));
        }

        public async Task<GeneralResponse> SearchLibraryAsync(SearchLibraryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
                return GeneralResponse.BadRequest("نص البحث مطلوب.");

            var books = await GetScopedBooksAsync();
            var results = (await MapBooksAsync(books))
                .Where(b => Matches(b.Title, request.Query)
                         || Matches(b.Description, request.Query)
                         || Matches(b.CoverImageUrl, request.Query)
                         || Matches(b.Author, request.Query))
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return GeneralResponse.Ok("تم إرجاع نتائج البحث بنجاح.", results, totalCount: results.Count);
        }

        public async Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request)
        {
            var pageNumber = request?.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request?.PageSize > 0 ? request.PageSize : 20;

            var books = await GetScopedBooksAsync();

            if (!string.IsNullOrWhiteSpace(request?.Category))
                books = books.Where(b => Matches(b.title, request.Category) || Matches(b.description, request.Category)).ToList();

            if (!string.IsNullOrWhiteSpace(request?.Author))
                books = books.Where(b => Matches(b.uploaded_by, request.Author)).ToList();

            if (!string.IsNullOrWhiteSpace(request?.Language))
                books = books.Where(b => Matches(b.description, request.Language) || Matches(b.FileUrl, request.Language)).ToList();

            var totalCount = books.Count;
            var items = (await MapBooksAsync(books))
                .OrderByDescending(b => b.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return GeneralResponse.Ok("تم جلب الكتب بنجاح.", items, pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetBookByIdAsync(GetBookByIdRequest request)
        {
            if (request == null || request.BookId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الكتاب مطلوب.");

            var book = await _unitOfWork.Repository<PdfFile>().GetByIdAsync(request.BookId);
            if (book == null)
                return GeneralResponse.NotFound("الكتاب غير موجود.");

            if (!await CanAccessBookAsync(book))
                return GeneralResponse.Unauthorized("لا يمكنك الوصول إلى كتاب تابع لمسجد آخر.");

            return GeneralResponse.Ok("تم جلب بيانات الكتاب بنجاح.", await MapBookAsync(book));
        }

        public async Task<GeneralResponse> GetBooksByCategoryAsync(GetBooksByCategoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Category))
                return GeneralResponse.BadRequest("التصنيف مطلوب.");

            var books = await GetScopedBooksAsync();
            var results = (await MapBooksAsync(books))
                .Where(b => Matches(b.Title, request.Category) || Matches(b.Description, request.Category))
                .OrderByDescending(b => b.CreatedAt)
                .ToList();

            return GeneralResponse.Ok("تم جلب الكتب حسب التصنيف بنجاح.", results, totalCount: results.Count);
        }

        public async Task<GeneralResponse> UpdateBookCategoriesAsync(UpdateBookCategoriesRequest request)
        {
            return GeneralResponse.BadRequest("تحديث تصنيفات الكتب غير مدعوم في هذا الإصدار.");
        }

        public async Task<GeneralResponse> DeleteBookAsync(DeleteBookRequest request)
        {
            if (request == null || request.BookId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الكتاب مطلوب.");

            if (!await CanManageLibraryAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بحذف الكتب.");

            var book = await _unitOfWork.Repository<PdfFile>().GetByIdAsync(request.BookId);
            if (book == null)
                return GeneralResponse.NotFound("الكتاب غير موجود.");

            if (!await CanAccessBookAsync(book))
                return GeneralResponse.Unauthorized("لا يمكنك الوصول إلى كتاب تابع لمسجد آخر.");

            await _unitOfWork.Repository<PdfFile>().DeleteAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف الكتاب بنجاح.", new { BookId = request.BookId });
        }

        private async Task<List<PdfFile>> GetScopedBooksAsync()
        {
            var books = (await _unitOfWork.Repository<PdfFile>().GetAllAsync()).ToList();
            if (_currentUserService.IsAdmin == true)
                return books;

            var mosqueId = await ResolveMosqueIdAsync();
            return mosqueId == Guid.Empty
                ? new List<PdfFile>()
                : books.Where(b => b.MosqueId == mosqueId).ToList();
        }

        private async Task<bool> CanAccessBookAsync(PdfFile book)
        {
            if (_currentUserService.IsAdmin == true)
                return true;

            var mosqueId = await ResolveMosqueIdAsync();
            return mosqueId != Guid.Empty && book.MosqueId == mosqueId;
        }

        private async Task<bool> CanManageLibraryAsync()
        {
            if (_currentUserService.IsAdmin == true)
                return true;

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return false;

            var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
            return supervisor != null;
        }

        private async Task<(Guid MosqueId, Guid SaturdayLessonId)?> ResolveLibraryPlacementAsync()
        {
            var mosqueId = await ResolveMosqueIdAsync();
            if (mosqueId == Guid.Empty)
                return null;

            var saturdayHalqas = (await _unitOfWork.Repository<SaturdayHalqa>().GetAllAsync()).ToList();
            var saturdayLessons = (await _unitOfWork.Repository<SaturdayLesson>().GetAllAsync()).ToList();

            var lesson = saturdayLessons.FirstOrDefault(l =>
                saturdayHalqas.Any(h => h.Id == l.SaturdayHalqeId && h.MosqueId == mosqueId));

            if (lesson == null)
                return null;

            return (mosqueId, lesson.Id);
        }

        private async Task<Guid> ResolveMosqueIdAsync()
        {
            var currentUserId = _currentUserService.CurrentUserId;
            if (currentUserId.HasValue)
            {
                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor != null)
                    return supervisor.MosqueId;

                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(currentUserId.Value);
                if (teacher != null)
                    return teacher.MosqueId;

                var examiner = await _unitOfWork.Repository<TeacherExam>().GetByIdAsync(currentUserId.Value);
                if (examiner != null)
                    return examiner.MosquId;
            }

            if (_currentUserService.IsAdmin == true)
            {
                var mosque = (await _unitOfWork.Repository<Mosque>().GetAllAsync()).FirstOrDefault();
                return mosque?.Id ?? Guid.Empty;
            }

            if (!_currentUserService.IsInRole(Roles.Examer.ToString()))
                return Guid.Empty;

            var bookMosqueIds = (await _unitOfWork.Repository<PdfFile>().GetAllAsync())
                .Select(book => book.MosqueId)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .Take(2)
                .ToList();

            return bookMosqueIds.Count == 1 ? bookMosqueIds[0] : Guid.Empty;
        }

        private static bool Matches(string? source, string? query)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(query))
                return false;

            return source.Contains(query, StringComparison.OrdinalIgnoreCase);
        }

        private async Task<List<BookResponseDto>> MapBooksAsync(IEnumerable<PdfFile> books)
        {
            var items = books.ToList();
            var uploaderNames = await ResolveUploaderNamesAsync(items);

            return items.Select(book => MapBook(book, ResolveUploaderDisplayName(book.uploaded_by, uploaderNames))).ToList();
        }

        private async Task<BookResponseDto> MapBookAsync(PdfFile book)
        {
            var uploaderNames = await ResolveUploaderNamesAsync(new[] { book });
            return MapBook(book, ResolveUploaderDisplayName(book.uploaded_by, uploaderNames));
        }

        private async Task<Dictionary<Guid, string>> ResolveUploaderNamesAsync(IEnumerable<PdfFile> books)
        {
            var userIds = books
                .Select(book => Guid.TryParse(book.uploaded_by, out var userId) ? userId : Guid.Empty)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            if (userIds.Count == 0)
                return new Dictionary<Guid, string>();

            var users = (await _unitOfWork.Repository<User>().GetAllAsync())
                .Where(user => userIds.Contains(user.Id))
                .ToDictionary(user => user.Id, user => string.IsNullOrWhiteSpace(user.name) ? user.Id.ToString() : user.name.Trim());

            return users;
        }

        private async Task<string> ResolveCurrentUserDisplayNameAsync()
        {
            var claimedName = _currentUserService.CurrentUserName?.Trim();
            if (!string.IsNullOrWhiteSpace(claimedName))
                return claimedName;

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return "System";

            var user = await _unitOfWork.Repository<User>().GetByIdAsync(currentUserId.Value);
            return string.IsNullOrWhiteSpace(user?.name) ? "System" : user.name.Trim();
        }

        private static string ResolveUploaderDisplayName(string? storedUploader, IReadOnlyDictionary<Guid, string> uploaderNames)
        {
            if (string.IsNullOrWhiteSpace(storedUploader))
                return "غير محدد";

            if (Guid.TryParse(storedUploader, out var uploaderId) && uploaderNames.TryGetValue(uploaderId, out var uploaderName))
                return uploaderName;

            return storedUploader.Trim();
        }

        private static BookResponseDto MapBook(PdfFile book, string uploaderDisplayName)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.title,
                Author = uploaderDisplayName,
                ISBN = string.Empty,
                Publisher = string.Empty,
                PublicationYear = book.created_at.Year,
                Description = book.description,
                CoverImageUrl = book.FileUrl,
                CreatedAt = book.created_at,
                UpdatedAt = null
            };
        }

        private static string SanitizeFileName(string name)
        {
            var invalid = Path.GetInvalidFileNameChars();
            var clean = new string(name.Where(c => !invalid.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(clean) ? "book" : clean;
        }
    }
}
