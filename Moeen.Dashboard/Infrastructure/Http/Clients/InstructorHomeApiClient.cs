using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Responses.TeacherDashboard;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class InstructorHomeApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly ITokenService _tokenService;

        public InstructorHomeApiClient(HttpClient httpClient, ITokenService tokenService)
        {
            _httpClient = httpClient;
            _tokenService = tokenService;
        }

        public Task<TeacherDashboardOverviewResponse?> GetOverviewAsync()
            => GetAsync<TeacherDashboardOverviewResponse>(ApiRoutes.TeacherDashboardOverviewRoute);

        public Task<TeacherHalaqaProgressResponse?> GetMyHalaqasProgressAsync()
            => GetAsync<TeacherHalaqaProgressResponse>(ApiRoutes.TeacherHalaqasProgressRoute);

        private async Task<T?> GetAsync<T>(string route)
        {
            var token = await _tokenService.Get();
            if (string.IsNullOrWhiteSpace(token))
                throw new InvalidOperationException("انتهت جلسة تسجيل الدخول، يرجى تسجيل الدخول مرة أخرى.");

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.TryAddWithoutValidation("Author" + "ization", "Bear" + "er " + token);

            using var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
