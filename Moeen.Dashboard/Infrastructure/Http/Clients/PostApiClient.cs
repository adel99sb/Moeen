using Moeen.Shared.Requests;
using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ContentSharing;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class PostApiClient
    {
        private readonly HttpClient _http;
        private const string BaseRoute = "api/content-sharing"; // هاد اسم الـ Controller بالباك إند غالباً

        public PostApiClient(HttpClient http)
        {
            _http = http;
        }

        // 1. نشر بوست جديد
        public async Task<GeneralResponse> PublishPost(PublishPostRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/publish", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 2. تعديل بوست
        public async Task<GeneralResponse> UpdatePost(UpdatePostRequest request)
        {
            var response = await _http.PutAsJsonAsync($"{BaseRoute}/update", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 3. حذف بوست
        public async Task<GeneralResponse> DeletePost(DeletePostRequest request)
        {
            // نرسل الـ Request بجسم الطلب أو عبر الرابط حسب إعداد الـ API
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/delete", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 4. جلب بوست معين بواسطة الـ ID
        public async Task<GeneralResponse> GetPostById(GetPostByIdRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/get-by-id", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 5. جلب تفاعلات بوست (لايكات)
        public async Task<GeneralResponse> GetPostInteractions(GetPostInteractionsRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/interactions", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 6. التفاعل مع بوست (إضافة/إزالة لايك)
        public async Task<GeneralResponse> InteractWithPost(InteractWithPostRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/interact", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 7. إدارة الإعلانات
        public async Task<GeneralResponse> ManageAnnouncements(ManageAnnouncementRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/manage-announcements", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 8. البحث في المحتوى والبوستات
        public async Task<GeneralResponse> SearchContent(SearchContentRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/search", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 9. حذف المحتوى القديم
        public async Task<GeneralResponse> DeleteOldContent(DeleteOldContentRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/delete-old", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 10. إضافة وسائط متعددة للبوست
        public async Task<GeneralResponse> AddMultimedia(AddMultimediaRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BaseRoute}/add-multimedia", request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}