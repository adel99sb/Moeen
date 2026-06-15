using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ContentSharing;
using System.Text.Json;

namespace Moeen.Dashboard.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly PostApiClient _postApiClient;

        public PostService(PostApiClient postApiClient)
        {
            _postApiClient = postApiClient;
        }

        public async Task<List<PostDto>> GetAllPostsAsync()
        {
            var res = await _postApiClient.GetAllPosts();
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية جلب المنشورات");
            return ExtractPostList(res.Data);
        }

        public async Task<List<HalqaBriefDto>> GetHalqasBriefAsync(string? query = null)
        {
            var res = await _postApiClient.GetHalqasBrief(query);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية جلب الحلقات");
            return ExtractHalqaList(res.Data);
        }

        public async Task<GeneralResponse> PublishPostAsync(PublishPostRequest request)
        {
            var res = await _postApiClient.PublishPost(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية نشر المنشور");
            return res;
        }

        public async Task<GeneralResponse> UpdatePostAsync(UpdatePostRequest request)
        {
            var res = await _postApiClient.UpdatePost(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية تعديل المنشور");
            return res;
        }

        public async Task<GeneralResponse> DeletePostAsync(DeletePostRequest request)
        {
            var res = await _postApiClient.DeletePost(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية حذف المنشور");
            return res;
        }

        public async Task<GeneralResponse> GetPostByIdAsync(GetPostByIdRequest request)
        {
            var res = await _postApiClient.GetPostById(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية جلب المنشور");
            return res;
        }

        public async Task<GeneralResponse> GetPostInteractionsAsync(GetPostInteractionsRequest request)
        {
            var res = await _postApiClient.GetPostInteractions(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية جلب التفاعلات");
            return res;
        }

        public async Task<GeneralResponse> InteractWithPostAsync(InteractWithPostRequest request)
        {
            var res = await _postApiClient.InteractWithPost(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية التفاعل مع المنشور");
            return res;
        }

        public async Task<GeneralResponse> ManageAnnouncementsAsync(ManageAnnouncementRequest request)
        {
            var res = await _postApiClient.ManageAnnouncements(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية إدارة الإعلانات");
            return res;
        }

        public async Task<GeneralResponse> SearchContentPostAsync(SearchContentRequest request)
        {
            var res = await _postApiClient.SearchContentPost(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية البحث (POST)");
            return res;
        }

        public async Task<GeneralResponse> SearchContentGetAsync(string query)
        {
            var res = await _postApiClient.SearchContentGet(query);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية البحث (GET)");
            return res;
        }

        public async Task<GeneralResponse> DeleteOldContentAsync(DeleteOldContentRequest request)
        {
            var res = await _postApiClient.DeleteOldContent(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية حذف المحتوى القديم");
            return res;
        }

        public async Task<GeneralResponse> AddMultimediaAsync(AddMultimediaRequest request)
        {
            var res = await _postApiClient.AddMultimedia(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية إضافة الوسائط");
            return res;
        }

        private static List<HalqaBriefDto> ExtractHalqaList(object? data)
        {
            if (data == null)
                return new List<HalqaBriefDto>();

            if (data is IEnumerable<HalqaBriefDto> halqas)
                return halqas.ToList();

            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<List<HalqaBriefDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<HalqaBriefDto>();
        }

        private static List<PostDto> ExtractPostList(object? data)
        {
            if (data == null)
                return new List<PostDto>();

            if (data is IEnumerable<PostDto> posts)
                return posts.ToList();

            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<List<PostDto>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<PostDto>();
        }
    }
}
