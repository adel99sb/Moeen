using Moeen.Api.Shared.Requests.Mosuq;
using Moeen.Api.Shared.Responses.Mosuq;

namespace Moeen.Api.Core.Contracts.Application
{
    public interface IMosquService
    {
        Task<bool> AddMosqu(AddMosquReq req);
        Task<GetAllMosqusResponse> GetAllMosqus();
    }
}
