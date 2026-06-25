using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Registration;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Registration;

namespace Moeen.Api.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly AppDbContext _context;

        public RegistrationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> RegisterInCircleAsync(RegisterInCircleRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty || request.HalqaId == Guid.Empty)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var circle = await _context.Halqas.FindAsync(request.HalqaId);
            if (circle == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            if (student.HalqaId.HasValue && student.HalqaId != Guid.Empty)
            {
                if (student.HalqaId == request.HalqaId)
                    return GeneralResponse.BadRequest("الطالب مسجل بالفعل في هذه الحلقة.");

                return GeneralResponse.BadRequest("الطالب مسجل في حلقة أخرى، استخدم النقل.");
            }

            student.HalqaId = request.HalqaId;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تسجيل الطالب في الحلقة بنجاح.");
        }

        public async Task<GeneralResponse> UnregisterFromCircleAsync(UnregisterFromCircleRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty || request.CircleId == Guid.Empty)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            if (!student.HalqaId.HasValue || student.HalqaId == Guid.Empty)
                return GeneralResponse.BadRequest("الطالب غير مسجل في أي حلقة.");

            if (student.HalqaId != request.CircleId)
                return GeneralResponse.BadRequest("الطالب غير مسجل في هذه الحلقة.");

            student.HalqaId = null;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إلغاء تسجيل الطالب من الحلقة.");
        }

        public async Task<GeneralResponse> TransferStudentAsync(TransferStudentRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty || request.FromCircleId == Guid.Empty || request.ToCircleId == Guid.Empty)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            if (student.HalqaId != request.FromCircleId)
                return GeneralResponse.BadRequest("الطالب غير مسجل في الحلقة المصدر.");

            var targetCircle = await _context.Halqas.FindAsync(request.ToCircleId);
            if (targetCircle == null)
                return GeneralResponse.NotFound("الحلقة المستهدفة غير موجودة.");

            student.HalqaId = request.ToCircleId;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم نقل الطالب بنجاح.");
        }

        public async Task<GeneralResponse> GetCircleStudentsAsync(GetCircleRegisteredStudentsRequest request)
        {
            if (request == null || request.CircleId == Guid.Empty)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var circle = await _context.Halqas.FindAsync(request.CircleId);
            if (circle == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            int page = Math.Max(1, request.PageNumber);
            int pageSize = Math.Max(1, request.PageSize);

            var query = _context.Students
                .Where(s => s.HalqaId == request.CircleId && s.status == 0);

            if (!string.IsNullOrWhiteSpace(request.StudentName))
                query = query.Where(s => EF.Functions.Like(s.name, $"%{request.StudentName}%"));

            if (request.Status.HasValue && (int)request.Status.Value == 0)
                query = query.Where(s => s.status == (int)request.Status.Value);

            var totalCount = await query.CountAsync();

            var students = await query
                .OrderBy(s => s.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var data = students.Select(MapCircleStudentDto).ToList();

            return GeneralResponse.Ok("تم جلب طلاب الحلقة بنجاح.", data, page, pageSize, totalCount);
        }

        private static CircleStudentDto MapCircleStudentDto(Student student)
        {
            return new CircleStudentDto
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                Status = ResolveRegistrationStatus(student.status),
                RegisteredAt = student.EnrollmentDate
            };
        }

        private static RegistrationStatus ResolveRegistrationStatus(int status)
        {
            return Enum.IsDefined(typeof(RegistrationStatus), status)
                ? (RegistrationStatus)status
                : RegistrationStatus.Active;
        }
    }
}