using System.Net.Http.Json;
using System.Text.Json;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Complaint;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Complaint;

namespace Moeen.Dashboard.Services.Implementations
{
    public sealed class ComplaintApiService : IComplaintApiService
    {
        private readonly HttpClient _httpClient;

        public ComplaintApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IReadOnlyList<ComplaintResponse>> GetAllComplaintsAsync(CancellationToken cancellationToken = default)
        {
            var response = await _httpClient.GetFromJsonAsync<GeneralResponse>("api/Complaint", cancellationToken);
            if (response?.Data is not JsonElement element || element.ValueKind != JsonValueKind.Array)
            {
                return Array.Empty<ComplaintResponse>();
            }

            return element.Deserialize<List<ComplaintResponse>>(new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? [];
        }

        public async Task<string?> UpdateComplaintStatusAsync(Guid complaintId, ComplaintStatus status, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.PutAsJsonAsync($"api/Complaint/{complaintId}/status", new UpdateComplaintStatusRequest
            {
                Status = status
            }, cancellationToken);

            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message ?? (response.IsSuccessStatusCode ? "Complaint status updated successfully." : "Failed to update complaint status.");
        }

        public async Task<string?> DeleteComplaintAsync(Guid complaintId, CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.DeleteAsync($"api/Complaint/{complaintId}", cancellationToken);
            var body = await response.Content.ReadFromJsonAsync<GeneralResponse>(cancellationToken: cancellationToken);
            return body?.Message ?? (response.IsSuccessStatusCode ? "Complaint deleted successfully." : "Failed to delete complaint.");
        }
    }
}
