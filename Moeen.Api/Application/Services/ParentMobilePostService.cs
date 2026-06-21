using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.Api.Application.Services
{
    public class ParentMobilePostService : IParentMobilePostService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;

        public ParentMobilePostService(AppDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<GeneralResponse> GetPostsAsync(Guid parentId)
        {
            var parent = await GetParentAsync(parentId);
            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            var children = await GetChildrenAsync(parentId);
            if (children.Count == 0)
                return GeneralResponse.Ok("لا يوجد طلاب مرتبطون بحساب ولي الأمر.", new ParentPostResponse());

            var mosqueIds = children
                .Where(c => c.MosqueId != Guid.Empty)
                .Select(c => c.MosqueId)
                .Distinct()
                .ToList();

            var halqaIds = children
                .Where(c => c.HalqaId.HasValue)
                .Select(c => c.HalqaId!.Value)
                .Distinct()
                .ToList();

            var posts = await _context.Posts
                .AsNoTracking()
                .Include(p => p.Mosque)
                .Include(p => p.Halqa)
                .Include(p => p.PosInteractions)
                .Where(p =>
                    mosqueIds.Contains(p.MosqueId) &&
                    (!p.HalqaId.HasValue || halqaIds.Contains(p.HalqaId.Value)))
                .OrderByDescending(p => p.created_at)
                .Take(50)
                .ToListAsync();

            var response = new ParentPostResponse();
            foreach (var post in posts)
                response.Posts.Add(await MapPostAsync(post, parentId, children));

            return GeneralResponse.Ok("تم جلب منشورات ولي الأمر.", response);
        }

        public async Task<GeneralResponse> ToggleLikeAsync(Guid parentId, Guid postId)
        {
            var parent = await GetParentAsync(parentId);
            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            var children = await GetChildrenAsync(parentId);
            if (children.Count == 0)
                return GeneralResponse.Unauthorized("لا يوجد طلاب مرتبطون بحساب ولي الأمر الحالي.");

            var post = await _context.Posts
                .Include(p => p.PosInteractions)
                .FirstOrDefaultAsync(p => p.Id == postId);

            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            if (!CanParentSeePost(children, post))
                return GeneralResponse.Unauthorized("لا يمكنك التفاعل مع هذا المنشور لأنه غير مرتبط بأبنائك.");

            var existing = await _context.PosInteractions
                .FirstOrDefaultAsync(i => i.PostId == postId && i.UserId == parentId);

            var isLiked = existing == null;
            if (existing == null)
            {
                await _context.PosInteractions.AddAsync(new PosInteraction
                {
                    Id = Guid.NewGuid(),
                    PostId = postId,
                    UserId = parentId,
                    date = DateTime.UtcNow
                });
            }
            else
            {
                _context.PosInteractions.Remove(existing);
            }

            await _context.SaveChangesAsync();

            var count = await _context.PosInteractions.CountAsync(i => i.PostId == postId);
            return GeneralResponse.Ok("تم تحديث التفاعل مع المنشور.", new ParentPostInteractionResponse
            {
                PostId = postId,
                IsLikedByCurrentParent = isLiked,
                InteractionsCount = count
            });
        }

        private async Task<Student?> GetParentAsync(Guid parentId)
        {
            if (parentId == Guid.Empty)
                return null;

            return await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == parentId);
        }

        private async Task<List<Student>> GetChildrenAsync(Guid parentId)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.ParentId == parentId)
                .ToListAsync();
        }

        private static bool CanParentSeePost(List<Student> children, Post post)
        {
            return children.Any(child =>
                child.MosqueId == post.MosqueId &&
                (!post.HalqaId.HasValue || child.HalqaId == post.HalqaId));
        }

        private async Task<ParentPostDto> MapPostAsync(Post post, Guid parentId, List<Student> children)
        {
            return new ParentPostDto
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
                IsLikedByCurrentParent = post.PosInteractions?.Any(i => i.UserId == parentId) ?? false,
                VisibleForChildren = children
                    .Where(child => child.MosqueId == post.MosqueId && (!post.HalqaId.HasValue || child.HalqaId == post.HalqaId))
                    .Select(child => child.name ?? string.Empty)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct()
                    .ToList()
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
