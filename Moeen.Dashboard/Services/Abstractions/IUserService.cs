using Moeen.Shared.Responses;
using System;
using System.Threading.Tasks;

namespace Moeen.Dashboard.Services.Abstractions
{
    public interface IUserService
    {
        // هذه الميثود الوحيدة التي نحتاجها حالياً لجلب بيانات المعلم وعرضها بالبروفايل
        Task<GeneralResponse> GetUserByIdAsync(Guid userId);
    }
}