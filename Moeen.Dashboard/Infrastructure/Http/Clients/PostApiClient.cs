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
            var response = await _http.PostAsJsonAsync(ApiRoutes.PublishPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 2. تعديل بوست
        public async Task<GeneralResponse> UpdatePost(UpdatePostRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.ApdatePostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 3. حذف بوست
        public async Task<GeneralResponse> DeletePost(DeletePostRequest request)
        {
            // نرسل الـ Request بجسم الطلب أو عبر الرابط حسب إعداد الـ API
            var response = await _http.PostAsJsonAsync(ApiRoutes.DeletePostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 4. جلب بوست معين بواسطة الـ ID
        public async Task<GeneralResponse> GetPostById(GetPostByIdRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetPostByIdRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 5. جلب تفاعلات بوست (لايكات)
        public async Task<GeneralResponse> GetPostInteractions(GetPostInteractionsRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.GetInteractionPostByIdRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 6. التفاعل مع بوست (إضافة/إزالة لايك)
        public async Task<GeneralResponse> InteractWithPost(InteractWithPostRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.IntractPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 7. إدارة الإعلانات
        public async Task<GeneralResponse> ManageAnnouncements(ManageAnnouncementRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.ManagAnnoucmentRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 8-A. البحث عن طريق الـ POST (بياخد الـ Request كامل بـ الـ Body)
        public async Task<GeneralResponse> SearchContentPost(SearchContentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
        // 8-B. البحث عن طريق الـ GET (بيرسل كلمة البحث فوق بالرابط)
        public async Task<GeneralResponse> SearchContentGet(string query)
        {
            // من الصورة: الـ GET بيقرا من GetSearchPostRoute وبنربط معها الـ Query
            var url = $"{ApiRoutes.GetSearchPostRoute}?Query={Uri.EscapeDataString(query)}";

            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 9. حذف المحتوى القديم
        public async Task<GeneralResponse> DeleteOldContent(DeleteOldContentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.DeleteOldRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        // 10. إضافة وسائط متعددة للبوست
        public async Task<GeneralResponse> AddMultimedia(AddMultimediaRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AddMultiMediaRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
       
       
    }
}
