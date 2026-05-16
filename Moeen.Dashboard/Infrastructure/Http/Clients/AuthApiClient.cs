using Moeen.Shared.Requests.Enrollment;
using Moeen.Shared.Requests.Identity;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class AuthApiClient
    {
        private readonly HttpClient _http;

        public AuthApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse> Login(LoginRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.LoginRoute, request);

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();

            return content;
        }
        public async Task<GeneralResponse> Register(RegisterRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.RegisterRoute, request);

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();

            return content;
        }
        public async Task<GeneralResponse> Search(SearchMembersRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchRoute, request);
            // التحقق إذا الطلب نجح على مستوى الـ HTTP
            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse { Success = false, Message = "مشكلة في الاتصال بالسيرفر" };
            }

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();

            return content;
        }
        public async Task<GeneralResponse> SendVerifyEmailCode(SendVerifyEmailCodeRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SendVerifyEmailRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
        public async Task<GeneralResponse> VerifyEmail(VerifyEmailRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.VerifyEmailRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
        public async Task<GeneralResponse> GetPostById(Guid postId)
        {
            var route = ApiRoutes.GetPostRoute.Replace("{postId}", postId.ToString());
            var response = await _http.GetAsync(route);

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse { Success = false, Message = "مشكلة في الاتصال بالسيرفر أو المنشور غير موجود" };
            }

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return content;
        }
        public async Task<GeneralResponse> GetPostInteractionsAsync(Guid postId)
        {
            // هنا نادينا   (GetPostInterractionRoute) 
            var route = ApiRoutes.GetPostInterractionRoute.Replace("{postId}", postId.ToString());

            // منبعت طلب GET للسيرفر عشان نجيب التفاعلات
            var response = await _http.GetAsync(route);

            if (!response.IsSuccessStatusCode)
            {
                return new GeneralResponse { Success = false, Message = "تعذر جلب تفاعلات المنشور" };
            }

            var content = await response.Content.ReadFromJsonAsync<GeneralResponse>();
            return content;
        }
        public async Task<GeneralResponse> DeletePostAsync(Guid postId)
        {
            var url = ApiRoutes.DeletePostRoute.Replace("{postId}", postId.ToString());

            var response = await _http.DeleteAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<GeneralResponse>();
            }

            return new GeneralResponse { Success = false, Message = "فشل في حذف المنشور" };
        }
    }
}
