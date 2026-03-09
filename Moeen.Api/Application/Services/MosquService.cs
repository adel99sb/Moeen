using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.Mosuq;
using Moeen.Api.Shared.Responses.Mosuq;

namespace Moeen.Api.Application.Services
{
    public class MosquService : IMosquService
    {
        public Task<bool> AddMosqu(AddMosquReq req)
        {
            throw new NotImplementedException();
        }

        public Task<GetAllMosqusResponse> GetAllMosqus()
        {
            throw new NotImplementedException();
        }
    }
}
