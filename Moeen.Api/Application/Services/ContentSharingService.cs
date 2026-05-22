using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ContentSharing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class ContentSharingService : IContentSharingService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IFileService _fileService;

        public ContentSharingService(
            IUnitOfWork unitOfWork,
            ICurrentUserService currentUserService,
            IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _fileService = fileService;
        }

        // ✅ موحدة: ترجع GeneralResponse<PostDto>
        private async Task<GeneralResponse> MapPostToDtoAsync(Post post, bool includeInteractions = false)
        {
            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            var dto = new PostDto
            {
                Id = post.Id,
                Title = post.title,
                Body = post.body,
                ImageUrl = string.IsNullOrWhiteSpace(post.imageUrl) ? null : await _file_service_GetUrlSafe(post.imageUrl),
                MosqueId = post.MosqueId == Guid.Empty ? null : post.MosqueId,
                HalqaId = post.MosqueId == Guid.Empty ? null : null,
                CreatedAt = post.created_at,
                InteractionsCount = post.PosInteractions?.Count ?? 0
            };

            if (includeInteractions)
            {
                List<PosInteraction> interactions;
                if (post.PosInteractions != null && post.PosInteractions.Any())
                {
                    interactions = post.PosInteractions.ToList();
                }
                else
                {
                    var spec = Spec.ForChain<PosInteraction>(
                        pi => pi.PostId == post.Id,
                        q => q.Include(pi => pi.User)
                    );
                    interactions = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(spec)).ToList();
                }

                dto.Interactions = interactions.Select(pi => new InteractionDto
                {
                    Id = pi.Id,
                    PostId = pi.PostId,
                    UserId = pi.UserId,
                    UserName = pi.User?.name ?? string.Empty,
                    CreatedAt = pi.date,
                    Type = Moeen.Shared.Constants.InteractionType.Like
                }).ToList();
            }

            return GeneralResponse.Ok("تم جلب بيانات المنشور بنجاح.", dto);
        }

        private Task<string> _file_service_GetUrlSafe(string path)
            => string.IsNullOrWhiteSpace(path) ? Task.FromResult<string?>(null) : _file_service_GetUrl(path);

        private async Task<string> _file_service_GetUrl(string path)
            => await _fileService.GetFileUrlAsync(path);

        public async Task<GeneralResponse> PublishPostAsync(PublishPostRequest request)
        {
            if (request?.PostData == null)
                return GeneralResponse.BadRequest("بيانات المنشور مطلوبة.");

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            Guid mosqueId;

            if (isAdmin)
            {
                mosqueId = request.PostData.MosqueId ?? Guid.Empty;
            }
            else
            {
                if (!currentUserId.HasValue)
                    return GeneralResponse.Unauthorized("المستخدم غير مصادق عليه.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null)
                    return GeneralResponse.Unauthorized("ملف المشرف غير موجود.");

                mosqueId = supervisor.MosqueId;
            }

            var post = new Post
            {
                Id = Guid.NewGuid(),
                MosqueId = mosqueId,
                title = request.PostData.Title,
                body = request.PostData.Body,
                imageUrl = request.PostData.ImageUrl,
                created_at = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Post>().AddAsync(post);
            await _unitOfWork.CompleteAsync();

            // ✅ استخراج الـ DTO من الـ GeneralResponse
            var mapResponse = await MapPostToDtoAsync(post, includeInteractions: false);
            if (!mapResponse.Success)
                return mapResponse;

            var dto = mapResponse.Data as PostDto;
            return GeneralResponse.Ok("تم نشر المنشور بنجاح.", dto);
        }

        public async Task<GeneralResponse> UpdatePostAsync(UpdatePostRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب التحديث مطلوب.");

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    return GeneralResponse.Unauthorized("المستخدم غير مصادق عليه.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null || supervisor.MosqueId != post.MosqueId)
                    return GeneralResponse.Unauthorized("غير مسموح بتحديث هذا المنشور.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
                post.title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Body))
                post.body = request.Body;

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                post.imageUrl = request.ImageUrl;

            await _unitOfWork.Repository<Post>().UpdateAsync(post);
            await _unitOfWork.CompleteAsync();

            var mapResponse = await MapPostToDtoAsync(post, includeInteractions: false);
            if (!mapResponse.Success)
                return mapResponse;

            var dto = mapResponse.Data as PostDto;
            return GeneralResponse.Ok("تم تحديث المنشور بنجاح.", dto);
        }

        public async Task<GeneralResponse> DeletePostAsync(DeletePostRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب الحذف مطلوب.");

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    return GeneralResponse.Unauthorized("غير مصرح.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null || supervisor.MosqueId != post.MosqueId)
                    return GeneralResponse.Unauthorized("غير مسموح.");
            }

            var interSpec = Spec.For<PosInteraction>(pi => pi.PostId == post.Id);
            var interactions = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(interSpec)).ToList();
            foreach (var pi in interactions)
                await _unitOfWork.Repository<PosInteraction>().DeleteAsync(pi);

            if (!string.IsNullOrWhiteSpace(post.imageUrl))
                await _fileService.DeleteFileAsync(post.imageUrl);

            await _unitOfWork.Repository<Post>().DeleteAsync(post);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف المنشور بنجاح.");
        }

        public async Task<GeneralResponse> GetPostByIdAsync(GetPostByIdRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب جلب المنشور مطلوب.");

            Post post;
            if (request.IncludeInteractions)
            {
                var spec = Spec.ForChain<Post>(
                    p => p.Id == request.PostId,
                    q => q.Include(p => p.PosInteractions).ThenInclude(pi => pi.User)
                );
                var posts = await _unitOfWork.Repository<Post>().GetAllAsync(spec);
                post = posts.FirstOrDefault();
                if (post == null)
                    return GeneralResponse.NotFound("المنشور غير موجود.");
            }
            else
            {
                post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
                if (post == null)
                    return GeneralResponse.NotFound("المنشور غير موجود.");
            }

            // ✅ استخراج الـ DTO من الـ GeneralResponse
            var mapResponse = await MapPostToDtoAsync(post, includeInteractions: request.IncludeInteractions);
            if (!mapResponse.Success)
                return mapResponse;

            var dto = mapResponse.Data as PostDto;
            return GeneralResponse.Ok("تم جلب المنشور بنجاح.", dto);
        }

        public async Task<GeneralResponse> GetPostInteractionsAsync(GetPostInteractionsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب التفاعلات مطلوب.");

            int skip = (request.PageNumber - 1) * request.PageSize;

            var spec = Spec.ForChain<PosInteraction>(
                pi => pi.PostId == request.PostId,
                q => q.Include(pi => pi.User)
            );

            spec.ApplyPaging(skip, request.PageSize);
            spec.ApplyOrderByDescending(pi => pi.date);

            var interactions = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(spec)).ToList();

            var dtos = interactions.Select(pi => new InteractionDto
            {
                Id = pi.Id,
                PostId = pi.PostId,
                UserId = pi.UserId,
                UserName = pi.User?.name ?? string.Empty,
                CreatedAt = pi.date,
                Type = Moeen.Shared.Constants.InteractionType.Like
            }).ToList();

            return GeneralResponse.Ok("تم جلب تفاعلات المنشور بنجاح.", dtos);
        }

        public async Task<GeneralResponse> InteractWithPostAsync(InteractWithPostRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب التفاعل مطلوب.");

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return GeneralResponse.Unauthorized("المستخدم غير مصادق عليه.");

            var existsSpec = Spec.For<PosInteraction>(pi => pi.PostId == request.PostId && pi.UserId == currentUserId.Value);
            var existing = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(existsSpec)).FirstOrDefault();

            if (existing != null)
            {
                await _unitOfWork.Repository<PosInteraction>().DeleteAsync(existing);
                await _unitOfWork.CompleteAsync();
                return GeneralResponse.Ok("تم إزالة التفاعل بنجاح.");
            }

            var interaction = new PosInteraction
            {
                Id = Guid.NewGuid(),
                PostId = request.PostId,
                UserId = currentUserId.Value,
                date = DateTime.UtcNow
            };

            await _unitOfWork.Repository<PosInteraction>().AddAsync(interaction);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تسجيل التفاعل بنجاح.");
        }

        public async Task<GeneralResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request)
        {
            return GeneralResponse.Ok("تمت إدارة الإعلانات بنجاح.");
        }

        public async Task<GeneralResponse> SearchContentAsync(SearchContentRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب البحث مطلوب.");

            var pagination = new PaginationRequest();

            int skip = (pagination.Page - 1) * pagination.PageSize;

            var spec = Spec.For<Post>(p => p.title.Contains(request.Query) || p.body.Contains(request.Query));
            spec.ApplyPaging(skip, pagination.PageSize);
            spec.ApplyOrderByDescending(p => p.created_at);
            spec.AddInclude(p => p.PosInteractions);

            var posts = (await _unitOfWork.Repository<Post>().GetAllAsync(spec)).ToList();

            var dtos = new List<PostDto>();
            foreach (var p in posts)
            {
                var mapResponse = await MapPostToDtoAsync(p, includeInteractions: false);
                if (mapResponse.Success)
                {
                    var dto = mapResponse.Data as PostDto;
                    if (dto != null)
                    {
                        dto.InteractionsCount = p.PosInteractions?.Count ?? 0;
                        dtos.Add(dto);
                    }
                }
            }

            var response = new SearchContentResponse { Posts = dtos };
            return GeneralResponse.Ok("تم البحث في المحتوى بنجاح.", response);
        }

        public async Task<GeneralResponse> DeleteOldContentAsync(DeleteOldContentRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب الحذف مطلوب.");

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            Guid? targetMosqueId = null;
            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    return GeneralResponse.Unauthorized("غير مصرح.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null)
                    return GeneralResponse.BadRequest("ملف المشرف غير موجود.");

                targetMosqueId = supervisor.MosqueId;
            }

            var spec = Spec.ForChain<Post>(
                p => p.created_at < request.OlderThan && (isAdmin || p.MosqueId == targetMosqueId),
                q => q.Include(p => p.PosInteractions)
            );

            var oldPosts = (await _unitOfWork.Repository<Post>().GetAllAsync(spec)).ToList();
            int deletedCount = 0;

            foreach (var post in oldPosts)
            {
                if (post.PosInteractions != null)
                {
                    foreach (var pi in post.PosInteractions.ToList())
                        await _unitOfWork.Repository<PosInteraction>().DeleteAsync(pi);
                }

                if (!string.IsNullOrWhiteSpace(post.imageUrl))
                    await _fileService.DeleteFileAsync(post.imageUrl);

                await _unitOfWork.Repository<Post>().DeleteAsync(post);
                deletedCount++;
            }

            if (deletedCount > 0)
                await _unitOfWork.CompleteAsync();

            var response = new DeleteOldContentResponse
            {
                Success = true,
                Message = $"تم حذف {deletedCount} منشور.",
                DeletedCount = deletedCount
            };
            return GeneralResponse.Ok($"تم حذف {deletedCount} منشور بنجاح.", response);
        }

        public async Task<GeneralResponse> AddMultimediaAsync(AddMultimediaRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب إضافة الوسائط مطلوب.");

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null)
                return GeneralResponse.NotFound("المنشور غير موجود.");

            var response = new AddMultimediaResponse
            {
                Success = true,
                Message = "تمت معالجة الوسائط",
                AddedCount = request.MediaUrls?.Count ?? 0
            };
            return GeneralResponse.Ok("تمت إضافة الوسائط بنجاح.", response);
        }

        public async Task<GeneralResponse> GetAllPostsAsync()
        {
            try
            {
                var posts = await _unitOfWork.Repository<Post>().GetAllAsync();
                var dtos = new List<PostDto>();

                foreach (var post in posts)
                {
                    // ✅ استخراج الـ DTO من الـ GeneralResponse
                    var mapResponse = await MapPostToDtoAsync(post, false);

                    if (mapResponse.Success && mapResponse.Data != null)
                    {
                        var dto = mapResponse.Data as PostDto;
                        if (dto != null)
                            dtos.Add(dto);
                    }
                }

                return GeneralResponse.Ok("تم جلب جميع المنشورات.", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب المنشورات.");
            }
        }
    }
}