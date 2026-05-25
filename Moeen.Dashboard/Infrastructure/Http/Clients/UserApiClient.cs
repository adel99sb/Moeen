using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class UserApiClient
    {
        private readonly HttpClient _httpClient;

        public UserApiClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<GeneralResponse> GetUserByIdAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"api/user/{userId}");

            if (response.IsSuccessStatusCode)
            {
                // ✅ معالجة حالة الـ null
                return await response.Content.ReadFromJsonAsync<GeneralResponse>()
                       ?? GeneralResponse.InternalError("الرد فارغ من الخادم");
            }

            return GeneralResponse.BadRequest("فشل في جلب بيانات المستخدم");
        }
    }
}