using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class HalqaManagementService : IHalqaManagementService
    {
        private readonly HalqaApiClient _client;

        public HalqaManagementService(HalqaApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> GetAllAsync(Guid? mosqueId = null)
            => await _client.GetAllAsync(mosqueId);

        public async Task<GeneralResponse> CreateAsync(CreateHalqaRequest request)
            => await _client.CreateAsync(request);

        public async Task<GeneralResponse> UpdateAsync(UpdateHalqaRequest request)
            => await _client.UpdateAsync(request);

        public async Task<GeneralResponse> DeleteAsync(DeleteHalqaRequest request)
            => await _client.DeleteAsync(request);
    }
}
