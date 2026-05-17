using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class PostService : IPostService
    {
        private readonly PostApiClient _postApiClient;

        public PostService(PostApiClient postApiClient)
        {
            _postApiClient = postApiClient;
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

        public async Task<GeneralResponse> SearchContentAsync(SearchContentRequest request)
        {
            var res = await _postApiClient.SearchContent(request);
            if (res == null || !res.Success) throw new Exception(res?.Message ?? "فشلت عملية البحث عن المحتوى");
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
    }
}
