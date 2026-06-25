using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class StudentMobilePostService : IStudentMobilePostService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public StudentMobilePostService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<GeneralResponse> GetPostsAsync(Guid studentId)
        {
            var student = await GetStudentAsync(studentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Mosque)
                .Include(p => p.Halqa)
                .Include(p => p.PosInteractions)
                .Where(p =>
                    p.MosqueId == student.MosqueId &&
                    (!p.HalqaId.HasValue || p.HalqaId == student.HalqaId))
                .OrderByDescending(p => p.created_at)
                .Take(50)
                .ToListAsync();

            var response = new StudentPostResponse();
            foreach (var post in posts)
                response.Posts.Add(await MapPostAsync(post, studentId));

            return GeneralResponse.Ok("تم جلب منشورات الطالب.", response);
        }

        public async Task<GeneralResponse> ToggleLikeAsync(Guid studentId, Guid postId)
        {
            var student = await GetStudentAsync(studentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var post = await _context.Posts
                .Include(p => p.PosInteractions)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            if (!CanStudentSeePost(student, post))
                return GeneralResponse.Unauthorized("لا يمكنك التفاعل مع هذا المنشور.");

            var existing = await _context.PosInteractions
                .FirstOrDefaultAsync(i => i.PostId == postId && i.UserId == studentId);

            var isLiked = existing == null;
            if (existing == null)
            {
                await _context.PosInteractions.AddAsync(new PosInteraction
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = studentId,
                    date = DateTime.UtcNow
                });
            }
            else
            {
                _context.PosInteractions.Remove(existing);
            }

            await _context.SaveChangesAsync();

            var count = await _context.PosInteractions.CountAsync(i => i.PostId == postId);
            return GeneralResponse.Ok("تم تحديث التفاعل.", new StudentPostInteractionResponse
            {
                PostId = postId,
                IsLikedByCurrentStudent = isLiked,
                InteractionsCount = count
            });
        }

        private async Task<Student?> GetStudentAsync(Guid studentId)
        {
            if (studentId == Guid.Empty)
                return null;

            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == studentId && s.status == 0);
        }

        private static bool CanStudentSeePost(Student student, Post post)
            => post.MosqueId == student.MosqueId &&
               (!post.HalqaId.HasValue || post.HalqaId == student.HalqaId);

        private async Task<StudentPostDto> MapPostAsync(Post post, Guid studentId)
        {
            return new StudentPostDto
            {
                Id = post.Id,
                Title = post.title ?? string.Empty,
                Body = post.body ?? string.Empty,
                ImageUrl = await BuildImageUrlAsync(post.imageUrl),
                MosqueId = post.MosqueId == Guid.Empty ? null : post.MosqueId,
                HalqaId = post.HalqaId,
                MosqueName = post.Mosque?.name,
                HalqaName = post.Halqa?.Name,
                CreatedAt = post.created_at,
                IsAnnouncement = !post.HalqaId.HasValue,
                InteractionsCount = post.PosInteractions?.Count ?? 0,
                IsLikedByCurrentStudent = post.PosInteractions?.Any(i => i.UserId == studentId) ?? false
            };
        }

        private async Task<string?> BuildImageUrlAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                return imageUrl;

            return await _fileService.GetFileUrlAsync(imageUrl);
        }
    }
}
