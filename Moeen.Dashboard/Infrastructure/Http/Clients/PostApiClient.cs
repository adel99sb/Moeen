using Moeen.Shared.Requests.ContentSharing;
using Moeen.Shared.Responses;
using System.Net.Http.Json;

namespace Moeen.Dashboard.Infrastructure.Http.Clients
{
    public class PostApiClient
    {
        private readonly HttpClient _http;

        public PostApiClient(HttpClient http)
        {
            _http = http;
        }

        public async Task<GeneralResponse?> GetAllPosts()
        {
            var response = await _http.GetAsync(ApiRoutes.GetAllPostsRoute);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> PublishPost(PublishPostRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.PublishPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> GetHalqasBrief(string? query)
        {
            var url = ApiRoutes.GetPostHalqasBriefRoute;
            if (!string.IsNullOrWhiteSpace(query))
                url += $"?query={Uri.EscapeDataString(query)}";

            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> UpdatePost(UpdatePostRequest request)
        {
            var response = await _http.PutAsJsonAsync(ApiRoutes.UpdatePostRoute(request.PostId), request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> DeletePost(DeletePostRequest request)
        {
            var response = await _http.DeleteAsync(ApiRoutes.DeletePostRoute(request.PostId));
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> GetPostById(GetPostByIdRequest request)
        {
            var response = await _http.GetAsync(ApiRoutes.GetPostByIdRoute(request.PostId, request.IncludeInteractions));
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> GetPostInteractions(GetPostInteractionsRequest request)
        {
            var response = await _http.GetAsync(
                ApiRoutes.GetPostInteractionsRoute(request.PostId, request.PageNumber, request.PageSize));
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> InteractWithPost(InteractWithPostRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.IntractPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> ManageAnnouncements(ManageAnnouncementRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.ManagAnnoucmentRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> SearchContentPost(SearchContentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.SearchPostRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> SearchContentGet(string query)
        {
            var url = $"{ApiRoutes.GetSearchPostRoute}?query={Uri.EscapeDataString(query ?? string.Empty)}";
            var response = await _http.GetAsync(url);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> DeleteOldContent(DeleteOldContentRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.DeleteOldRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }

        public async Task<GeneralResponse?> AddMultimedia(AddMultimediaRequest request)
        {
            var response = await _http.PostAsJsonAsync(ApiRoutes.AddMultiMediaRoute, request);
            return await response.Content.ReadFromJsonAsync<GeneralResponse>();
        }
    }
}
