using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.Library;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class LibraryService : ILibraryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public LibraryService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> AddBookAsync(AddBookRequest request)
        {
            if (request?.BookData == null)
                return GeneralResponse.BadRequest("بيانات الكتاب مطلوبة.");

            if (string.IsNullOrWhiteSpace(request.BookData.Title))
                return GeneralResponse.BadRequest("اسم الكتاب مطلوب.");

            if (string.IsNullOrWhiteSpace(request.BookData.Description))
                return GeneralResponse.BadRequest("وصف الكتاب مطلوب.");

            if (string.IsNullOrWhiteSpace(request.BookData.CoverImageUrl))
                return GeneralResponse.BadRequest("رابط التحميل مطلوب.");

            if (!await CanManageLibraryAsync())
                return GeneralResponse.Unauthorized("غير مصرح لك بإضافة الكتب.");

            var placement = await ResolveLibraryPlacementAsync();
            if (placement is null)
                return GeneralResponse.BadRequest("لا توجد بيانات مسجد/درس سبت مرتبطة لحفظ الكتاب.");

            var book = new PdfFile
            {
                Id = Guid.NewGuid(),
                MosqueId = placement.Value.MosqueId,
                SaturdayLessonId = placement.Value.SaturdayLessonId,
                FileUrl = request.BookData.CoverImageUrl.Trim(),
                title = request.BookData.Title.Trim(),
                description = request.BookData.Description.Trim(),
                uploaded_by = string.IsNullOrWhiteSpace(_currentUserService.CurrentUserName)
                    ? "System"
                    : _currentUserService.CurrentUserName.Trim(),
                created_at = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PdfFile>().AddAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تمت إضافة الكتاب بنجاح.", MapBook(book));
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

            if (!string.IsNullOrWhiteSpace(request.BookData.Title))
                book.title = request.BookData.Title.Trim();

            if (!string.IsNullOrWhiteSpace(request.BookData.Description))
                book.description = request.BookData.Description.Trim();

            if (!string.IsNullOrWhiteSpace(request.BookData.CoverImageUrl))
                book.FileUrl = request.BookData.CoverImageUrl.Trim();

            await _unitOfWork.Repository<PdfFile>().UpdateAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تحديث بيانات الكتاب بنجاح.", MapBook(book));
        }

        public async Task<GeneralResponse> SearchLibraryAsync(SearchLibraryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Query))
                return GeneralResponse.BadRequest("نص البحث مطلوب.");

            var books = (await _unitOfWork.Repository<PdfFile>().GetAllAsync()).ToList();
            var results = books
                .Where(b => Matches(b.title, request.Query)
                         || Matches(b.description, request.Query)
                         || Matches(b.FileUrl, request.Query)
                         || Matches(b.uploaded_by, request.Query))
                .OrderByDescending(b => b.created_at)
                .Select(MapBook)
                .ToList();

            return GeneralResponse.Ok("تم إرجاع نتائج البحث بنجاح.", results, totalCount: results.Count);
        }

        public async Task<GeneralResponse> GetAllBooksAsync(GetAllBooksRequest request)
        {
            var pageNumber = request?.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request?.PageSize > 0 ? request.PageSize : 20;

            var books = (await _unitOfWork.Repository<PdfFile>().GetAllAsync()).ToList();

            if (!string.IsNullOrWhiteSpace(request?.Category))
                books = books.Where(b => Matches(b.title, request.Category) || Matches(b.description, request.Category)).ToList();

            if (!string.IsNullOrWhiteSpace(request?.Author))
                books = books.Where(b => Matches(b.uploaded_by, request.Author)).ToList();

            if (!string.IsNullOrWhiteSpace(request?.Language))
                books = books.Where(b => Matches(b.description, request.Language) || Matches(b.FileUrl, request.Language)).ToList();

            var totalCount = books.Count;
            var items = books
                .OrderByDescending(b => b.created_at)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(MapBook)
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

            return GeneralResponse.Ok("تم جلب بيانات الكتاب بنجاح.", MapBook(book));
        }

        public async Task<GeneralResponse> GetBooksByCategoryAsync(GetBooksByCategoryRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Category))
                return GeneralResponse.BadRequest("التصنيف مطلوب.");

            var books = (await _unitOfWork.Repository<PdfFile>().GetAllAsync()).ToList();
            var results = books
                .Where(b => Matches(b.title, request.Category) || Matches(b.description, request.Category))
                .OrderByDescending(b => b.created_at)
                .Select(MapBook)
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

            await _unitOfWork.Repository<PdfFile>().DeleteAsync(book);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف الكتاب بنجاح.", new { BookId = request.BookId });
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
                saturdayHalqas.Any(h => h.Id == l.SaturdayHalqeId && h.MosqueId == mosqueId))
                ?? saturdayLessons.FirstOrDefault();

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
            }

            var mosque = (await _unitOfWork.Repository<Mosque>().GetAllAsync()).FirstOrDefault();
            return mosque?.Id ?? Guid.Empty;
        }

        private static bool Matches(string? source, string? query)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(query))
                return false;

            return source.Contains(query, StringComparison.OrdinalIgnoreCase);
        }

        private static BookResponseDto MapBook(PdfFile book)
        {
            return new BookResponseDto
            {
                Id = book.Id,
                Title = book.title,
                Author = book.uploaded_by,
                ISBN = string.Empty,
                Publisher = string.Empty,
                PublicationYear = book.created_at.Year,
                Description = book.description,
                CoverImageUrl = book.FileUrl,
                CreatedAt = book.created_at,
                UpdatedAt = null
            };
        }
    }
}