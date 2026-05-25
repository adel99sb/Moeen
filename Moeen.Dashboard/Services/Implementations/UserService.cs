using System;
using System.Threading.Tasks;
using Moeen.Dashboard.Infrastructure.Http.Clients;
using Moeen.Dashboard.Services.Abstractions;
using Moeen.Shared.Responses;

namespace Moeen.Dashboard.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserApiClient _userApiClient;

        public UserService(UserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }

        public async Task<GeneralResponse> GetUserByIdAsync(Guid userId)
        {
            // استدعاء الـ Client لجلب البيانات وتمريرها فوراً للواجهة
            return await _userApiClient.GetUserByIdAsync(userId);
        }
    }
}