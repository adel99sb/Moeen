using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Shared.Requests.CircleTeacherAssignment;
using Moeen.Api.Shared.Responses.CircleTeacherAssignment;
using Moeen.Api.Core.Entities;
using Moeen.Api.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moeen.Api.infrastructure.Repositories;

namespace Moeen.Api.Application.Services
{
    public class CircleTeacherAssignmentService : ICircleTeacherAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CircleTeacherAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<OperationResponseDto> AssignTeacherToCircleAsync(AssignTeacherToCircleRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                return new OperationResponseDto { Success = false, Message = "الحلقة غير موجودة." };

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
            if (teacher == null)
                return new OperationResponseDto { Success = false, Message = "المعلم غير موجود." };

            if (halqa.TeacherId == request.TeacherId)
                return new OperationResponseDto { Success = true, Message = "المعلم معين بالفعل على هذه الحلقة." };

            halqa.TeacherId = request.TeacherId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            return new OperationResponseDto { Success = true, Message = "تم تعيين المعلم للحلقة بنجاح." };
        }

        public async Task<OperationResponseDto> RemoveTeacherFromCircleAsync(RemoveTeacherFromCircleRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                return new OperationResponseDto { Success = false, Message = "الحلقة غير موجودة." };

            if (halqa.TeacherId != request.TeacherId)
                return new OperationResponseDto { Success = false, Message = "المعلم غير معين لهذه الحلقة." };

            // النموذج الحالي لا يدعم تعيين ثانوي/قابلية null، لذا نوجّه المستخدم لإعادة التعيين
            return new OperationResponseDto
            {
                Success = false,
                Message = "لا يمكن إزالة المعلم مباشرةً. الرجاء استبداله بمعلم آخر أولاً."
            };
        }

        public async Task<List<CircleAssignmentDto>> GetCirclesByTeacherAsync(GetCirclesByTeacherRequest request)
        {
            // تجنب N+1: أطلب Halqa مع Teacher عبر Include عند المصدر
            var spec = Spec.For<Halqa>(h => h.TeacherId == request.TeacherId, h => h.Teacher);
            var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(spec)).ToList();

            var result = new List<CircleAssignmentDto>(halqas.Count);
            foreach (var h in halqas)
            {
                result.Add(new CircleAssignmentDto
                {
                    CircleId = h.Id,
                    CircleName = h.Name,
                    TeacherId = h.TeacherId,
                    TeacherName = h.Teacher?.name ?? string.Empty, // Teacher محمّل مسبقاً
                    IsPrimary = true,
                    IsActive = true,
                    AssignedAt = DateTime.UtcNow // لا يوجد حقل AssignedAt في الـ schema؛ نستخدم وقت البناء كقيمة مؤقتة
                });
            }

            return result;
        }

        public async Task<List<CircleAssignmentDto>> GetTeachersByCircleAsync(GetTeachersByCircleRequest request)
        {
            // جلب Halqa مع Teacher لتجنّب استعلام إضافي
            var spec = Spec.For<Halqa>(h => h.Id == request.CircleId, h => h.Teacher);
            var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(spec)).ToList();
            var halqa = halqas.FirstOrDefault();
            if (halqa == null)
                return new List<CircleAssignmentDto>();

            var teacher = halqa.Teacher; // محمّل عبر Include

            var dto = new CircleAssignmentDto
            {
                CircleId = halqa.Id,
                CircleName = halqa.Name,
                TeacherId = halqa.TeacherId,
                TeacherName = teacher?.name ?? string.Empty,
                IsPrimary = true,
                IsActive = true,
                AssignedAt = DateTime.UtcNow
            };

            return new List<CircleAssignmentDto> { dto };
        }

        public async Task<OperationResponseDto> ReplaceTeacherInCircleAsync(ReplaceTeacherRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                return new OperationResponseDto { Success = false, Message = "الحلقة غير موجودة." };

            if (halqa.TeacherId != request.OldTeacherId)
                return new OperationResponseDto { Success = false, Message = "المعلم القديم غير معوّن حالياً لهذه الحلقة." };

            var newTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.NewTeacherId);
            if (newTeacher == null)
                return new OperationResponseDto { Success = false, Message = "المعلم الجديد غير موجود." };

            halqa.TeacherId = request.NewTeacherId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            return new OperationResponseDto { Success = true, Message = "تم استبدال المعلم بنجاح." };;
        }
    }
}
