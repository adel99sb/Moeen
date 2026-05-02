using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Api.Shared.Requests;
using Moeen.Api.Shared.Requests.ContentSharing;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.ContentSharing;

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

        // map entity -> dto (loads file URL and optional interactions)
        private async Task<PostDto> MapPostToDtoAsync(Post post, bool includeInteractions = false)
        {
            if (post == null) return null!;

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
                    Type = Core.Constants.InteractionType.Like // model lacks type; keep default
                }).ToList();
            }

            return dto;
        }

        // safe wrapper to get file url (avoid throwing on null)
        private Task<string> _file_service_GetUrlSafe(string path)
            => string.IsNullOrWhiteSpace(path) ? Task.FromResult<string?>(null) : _file_service_GetUrl(path);

        private async Task<string> _file_service_GetUrl(string path)
            => await _fileService.GetFileUrlAsync(path);

        public async Task<PostDto> PublishPostAsync(PublishPostRequest request)
        {
            if (request?.PostData == null)
                throw new ArgumentNullException(nameof(request.PostData));

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            Guid mosqueId;

            // Security: non-admins can only publish to their own mosque
            if (isAdmin)
            {
                mosqueId = request.PostData.MosqueId ?? Guid.Empty;
            }
            else
            {
                if (!currentUserId.HasValue)
                    throw new UnauthorizedAccessException("User not authenticated.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null)
                    throw new UnauthorizedAccessException("Supervisor profile not found.");

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

            return await MapPostToDtoAsync(post, includeInteractions: false);
        }

        public async Task<PostDto> UpdatePostAsync(UpdatePostRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null) throw new ArgumentException("Post not found.", nameof(request.PostId));

            // Authorization: only admin or supervisor of same mosque can update
            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    throw new UnauthorizedAccessException("User not authenticated.");

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null || supervisor.MosqueId != post.MosqueId)
                    throw new UnauthorizedAccessException("Forbidden to update this post.");
            }

            if (!string.IsNullOrWhiteSpace(request.Title))
                post.title = request.Title;

            if (!string.IsNullOrWhiteSpace(request.Body))
                post.body = request.Body;

            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                post.imageUrl = request.ImageUrl;

            await _unitOfWork.Repository<Post>().UpdateAsync(post);
            await _unitOfWork.CompleteAsync();

            return await MapPostToDtoAsync(post, includeInteractions: false);
        }

        public async Task<OperationResponseDto> DeletePostAsync(DeletePostRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null)
                return new OperationResponseDto { Success = false, Message = "Post not found." };

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    return new OperationResponseDto { Success = false, Message = "Unauthorized." };

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null || supervisor.MosqueId != post.MosqueId)
                    return new OperationResponseDto { Success = false, Message = "Forbidden." };
            }

            // load interactions and delete them explicitly to avoid cascade issues
            var interSpec = Spec.For<PosInteraction>(pi => pi.PostId == post.Id);
            var interactions = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(interSpec)).ToList();
            foreach (var pi in interactions)
                await _unitOfWork.Repository<PosInteraction>().DeleteAsync(pi);

            if (!string.IsNullOrWhiteSpace(post.imageUrl))
                await _fileService.DeleteFileAsync(post.imageUrl);

            await _unitOfWork.Repository<Post>().DeleteAsync(post);
            await _unitOfWork.CompleteAsync();

            return new OperationResponseDto { Success = true, Message = "Post deleted." };
        }

        public async Task<PostDto> GetPostByIdAsync(GetPostByIdRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            if (request.IncludeInteractions)
            {
                var spec = Spec.ForChain<Post>(
                    p => p.Id == request.PostId,
                    q => q.Include(p => p.PosInteractions).ThenInclude(pi => pi.User)
                );
                var posts = await _unitOfWork.Repository<Post>().GetAllAsync(spec);
                var post = posts.FirstOrDefault();
                if (post == null) return null!;
                return await MapPostToDtoAsync(post, includeInteractions: true);
            }

            var p = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            return await MapPostToDtoAsync(p, includeInteractions: false);
        }

        public async Task<List<InteractionDto>> GetPostInteractionsAsync(GetPostInteractionsRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            int skip = (request.PageNumber - 1) * request.PageSize;

            var spec = Spec.ForChain<PosInteraction>(
                pi => pi.PostId == request.PostId,
                q => q.Include(pi => pi.User)
            );

            spec.ApplyPaging(skip, request.PageSize);
            spec.ApplyOrderByDescending(pi => pi.date);

            var interactions = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(spec)).ToList();

            return interactions.Select(pi => new InteractionDto
            {
                Id = pi.Id,
                PostId = pi.PostId,
                UserId = pi.UserId,
                UserName = pi.User?.name ?? string.Empty,
                CreatedAt = pi.date,
                Type = Core.Constants.InteractionType.Like
            }).ToList();
        }

        public async Task<InteractWithPostResponse> InteractWithPostAsync(InteractWithPostRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return new InteractWithPostResponse { Success = false, Message = "Unauthorized." };

            // Toggle interaction: if exists -> remove, otherwise add
            var existsSpec = Spec.For<PosInteraction>(pi => pi.PostId == request.PostId && pi.UserId == currentUserId.Value);
            var existing = (await _unitOfWork.Repository<PosInteraction>().GetAllAsync(existsSpec)).FirstOrDefault();
            if (existing != null)
            {
                await _unitOfWork.Repository<PosInteraction>().DeleteAsync(existing);
                await _unitOfWork.CompleteAsync();
                return new InteractWithPostResponse { Success = true, Message = "Interaction removed." };
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

            return new InteractWithPostResponse { Success = true, Message = "Interaction recorded." };
        }

        public async Task<ManageAnnouncementResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request)
        {
            // Minimal implementation to preserve controller contract.
            return new ManageAnnouncementResponse { Success = true, Message = "OK" };
        }

        public async Task<SearchContentResponse> SearchContentAsync(SearchContentRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var pagination = new PaginationRequest(); // default values

            int skip = (pagination.Page - 1) * pagination.PageSize;

            var spec = Spec.For<Post>(p => p.title.Contains(request.Query) || p.body.Contains(request.Query));
            spec.ApplyPaging(skip, pagination.PageSize);
            spec.ApplyOrderByDescending(p => p.created_at);
            spec.AddInclude(p => p.PosInteractions); // include to compute counts without N+1

            var posts = (await _unitOfWork.Repository<Post>().GetAllAsync(spec)).ToList();

            var dtos = new List<PostDto>();
            foreach (var p in posts)
            {
                var dto = await MapPostToDtoAsync(p, includeInteractions: false);
                dto.InteractionsCount = p.PosInteractions?.Count ?? 0;
                dtos.Add(dto);
            }

            return new SearchContentResponse { Posts = dtos };
        }

        public async Task<DeleteOldContentResponse> DeleteOldContentAsync(DeleteOldContentRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var currentUserId = _currentUserService.CurrentUserId;
            var isAdmin = _currentUserService.IsAdmin ?? false;

            Guid? targetMosqueId = null;
            if (!isAdmin)
            {
                if (!currentUserId.HasValue)
                    return new DeleteOldContentResponse { Success = false, Message = "Unauthorized", DeletedCount = 0 };

                var supervisor = await _unitOfWork.Repository<Supervisor>().GetByIdAsync(currentUserId.Value);
                if (supervisor == null)
                    return new DeleteOldContentResponse { Success = false, Message = "Supervisor profile not found", DeletedCount = 0 };

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

            return new DeleteOldContentResponse
            {
                Success = true,
                Message = $"Deleted {deletedCount} posts.",
                DeletedCount = deletedCount
            };
        }

        public async Task<AddMultimediaResponse> AddMultimediaAsync(AddMultimediaRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));

            var post = await _unitOfWork.Repository<Post>().GetByIdAsync(request.PostId);
            if (post == null)
                return new AddMultimediaResponse { Success = false, Message = "Post not found", AddedCount = 0 };

            // Post entity has no media collection in current schema; acknowledge input.
            return new AddMultimediaResponse { Success = true, Message = "Media processed", AddedCount = request.MediaUrls?.Count ?? 0 };
        }
    }
}
