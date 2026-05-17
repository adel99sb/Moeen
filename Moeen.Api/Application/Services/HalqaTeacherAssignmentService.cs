using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.CircleTeacherAssignment;
using Moeen.Shared.Requests.HalqaTeacherAssignment;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.HalqaTeacherAssignment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class HalqaTeacherAssignmentService : IHalqaTeacherAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HalqaTeacherAssignmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GeneralResponse> AssignTeacherToHalqaAsync(AssignTeacherToHalqaRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("بيانات التعيين غير مكتملة.");

                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null)
                    return GeneralResponse.NotFound("المعلم غير موجود.");

                var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
                if (halqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");

                halqa.TeacherId = request.TeacherId;
                await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("تم تعيين المعلم للحلقة بنجاح.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء تعيين المعلم.", ex.Message);
            }
        }

        public async Task<GeneralResponse> RemoveTeacherFromHalqaAsync(RemoveTeacherFromHalqaRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("بيانات الإزالة غير مكتملة.");

                var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
                if (halqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");

                if (halqa.TeacherId != request.TeacherId)
                    return GeneralResponse.BadRequest("المعلم غير مرتبط بهذه الحلقة.");

                halqa.TeacherId = null;
                await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("تم إزالة المعلم من الحلقة بنجاح.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء إزالة المعلم.", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetHalqasByTeacherAsync(GetHalqasByTeacherRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرف المعلم غير صالح.");

                var spec = Spec.For<Halqa>(h => h.TeacherId == request.TeacherId);
                var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(spec)).ToList();

                var dtos = halqas.Select(h => new HalqaAssignmentDto
                {
                    HalqaId = h.Id,
                    HalqaName = h.Name,
                    TeacherId = (Guid)h.TeacherId,
                    TeacherName = h.Teacher?.name
                }).ToList();

                return GeneralResponse.Ok("تم جلب الحلقات المسندة للمعلم بنجاح.", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب الحلقات.", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetTeachersByHalqaAsync(GetTeachersByHalqaRequest request)
        {
            try
            {
                if (request == null || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرف الحلقة غير صالح.");

                var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
                if (halqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");
                // ✅ الطريقة الصحيحة:
                var teacher = halqa.TeacherId != Guid.Empty
                    ? await _unitOfWork.Repository<Teacher>().GetByIdAsync((Guid)halqa.TeacherId)
                    : null;
                var dtos = new List<TeacherAssignmentDto>();
                if (teacher != null)
                {
                    dtos.Add(new TeacherAssignmentDto
                    {
                        TeacherId = teacher.Id,
                        TeacherName = teacher.name,
                        HalqaId = halqa.Id,
                        HalqaName = halqa.Name
                    });
                }

                return GeneralResponse.Ok("تم جلب المعلمين المسندين للحلقة بنجاح.", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب المعلمين.", ex.Message);
            }
        }
      
        public async Task<GeneralResponse> ReplaceTeacherInHalqaAsync(ReplaceTeacherRequest request)
        {
            try
            {
                if (request == null || request.OldTeacherId == Guid.Empty || request.NewTeacherId == Guid.Empty || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("بيانات الاستبدال غير مكتملة.");

                var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.HalqaId);
                if (halqa == null)
                    return GeneralResponse.NotFound("الحلقة غير موجودة.");

                if (halqa.TeacherId != request.OldTeacherId)
                    return GeneralResponse.BadRequest("المعلم الحالي غير مرتبط بهذه الحلقة.");

                var newTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.NewTeacherId);
                if (newTeacher == null)
                    return GeneralResponse.NotFound("المعلم الجديد غير موجود.");

                halqa.TeacherId = request.NewTeacherId;
                await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("تم استبدال المعلم بنجاح.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء استبدال المعلم.", ex.Message);
            }
        }

       
        
    }
}
