using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementations
{
    public class FoujManagementService : IFoujManagementService
    {
        private readonly FoujApiClient _client;

        public FoujManagementService(FoujApiClient client)
        {
            _client = client;
        }

        public async Task<GeneralResponse> GetAllAsync(GetAllFoujsRequest request)
            => await _client.GetAllAsync(request);

        public async Task<GeneralResponse> CreateAsync(CreateFoujRequest request)
            => await _client.CreateAsync(request);

        public async Task<GeneralResponse> UpdateAsync(UpdateFoujRequest request)
            => await _client.UpdateAsync(request);

        public async Task<GeneralResponse> DeleteAsync(Guid foujId)
            => await _client.DeleteAsync(foujId);
    }
}
