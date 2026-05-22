using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses; // ✅ استيراد GeneralResponse
using Moeen.Shared.Responses.Halqa;
using System;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class HalqaCommandService : IHalqaCommandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HalqaCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResponse> CreateHalqaAsync(CreateHalqaRequest request)
        {
            // تحقق من وجود الفوج
            var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.FoujId);
            if (fouj == null)
                return GeneralResponse.BadRequest("الفوج غير موجود.");

            // تحقق من وجود المعلم
            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
            if (teacher == null)
                return GeneralResponse.BadRequest("المعلم غير موجود.");

            var halqa = new Halqa
            {
                Id = Guid.NewGuid(),
                FoujId = request.FoujId,
                Name = request.Name,
                TeacherId = request.TeacherId,
                Type = request.Type
            };

            await _unitOfWork.Repository<Halqa>().AddAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj?.name,
                TeacherId = (Guid)halqa.TeacherId,
                TeacherName = teacher?.name,
                Type = halqa.Type,
                StudentsCount = 0
            };

            return GeneralResponse.Ok("تم إنشاء الحلقة بنجاح.", dto);
        }

        public async Task<GeneralResponse> DeleteHalqaAsync(DeleteHalqaRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            await _unitOfWork.Repository<Halqa>().DeleteAsync(halqa);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف الحلقة بنجاح.");
        }

        public async Task<GeneralResponse> MoveToFoujAsync(MoveHalqaToFoujRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var targetFouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.TargetFoujId);
            if (targetFouj == null)
                return GeneralResponse.BadRequest("الفوج المستهدف غير موجود.");

            halqa.FoujId = request.TargetFoujId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync((Guid)halqa.TeacherId);

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = targetFouj?.name,
                TeacherId = (Guid)halqa.TeacherId,
                TeacherName = teacher?.name,
                Type = halqa.Type
            };

            return GeneralResponse.Ok("تم نقل الحلقة إلى الفوج بنجاح.", dto);
        }

        public async Task<GeneralResponse> ReassignTeacherAsync(ReassignHalqaTeacherRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var newTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.NewTeacherId);
            if (newTeacher == null)
                return GeneralResponse.BadRequest("المعلم الجديد غير موجود.");

            halqa.TeacherId = request.NewTeacherId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(halqa.FoujId);

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj?.name,
                TeacherId = (Guid)halqa.TeacherId,
                TeacherName = newTeacher?.name,
                Type = halqa.Type
            };

            return GeneralResponse.Ok("تم إعادة تعيين المعلم للحلقة بنجاح.", dto);
        }

        public async Task<GeneralResponse> UpdateHalqaAsync(UpdateHalqaRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                halqa.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Type))
                halqa.Type = request.Type;

            if (request.FoujId.HasValue)
            {
                var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.FoujId.Value);
                if (fouj == null)
                    return GeneralResponse.BadRequest("الفوج المستهدف غير موجود.");
                halqa.FoujId = request.FoujId.Value;
            }

            if (request.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId.Value);
                if (teacher == null)
                    return GeneralResponse.BadRequest("المعلم غير موجود.");
                halqa.TeacherId = request.TeacherId.Value;
            }

            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var resultFouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(halqa.FoujId);
            var resultTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync((Guid)halqa.TeacherId);

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = resultFouj?.name,
                TeacherId = (Guid)halqa.TeacherId,
                TeacherName = resultTeacher?.name,
                Type = halqa.Type
            };

            return GeneralResponse.Ok("تم تحديث الحلقة بنجاح.", dto);
        }
    }
}