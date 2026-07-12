using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Halqa;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Halqa;

namespace Moeen.Api.Application.Services
{
    public class HalqaCommandService : IHalqaCommandService
    {
        private const int StudentRole = 2;

        private readonly IUnitOfWork _unitOfWork;
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public HalqaCommandService(
            IUnitOfWork unitOfWork,
            AppDbContext context,
            ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> CreateHalqaAsync(CreateHalqaRequest request)
        {
            var fouj = await _context.Foujs.FirstOrDefaultAsync(f => f.Id == request.FoujId);
            if (fouj == null)
                return GeneralResponse.BadRequest("الفوج غير موجود.");

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == request.TeacherId);
            if (teacher == null)
                return GeneralResponse.BadRequest("المعلم غير موجود.");

            if (teacher.status != 0)
                return GeneralResponse.BadRequest("لا يمكن تعيين معلم غير نشط على حلقة.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue)
            {
                if (fouj.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك إنشاء حلقة خارج نطاق المسجد الذي تديره.");

                if (teacher.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك تعيين معلم خارج نطاق المسجد الذي تديره.");
            }

            var halqa = new Halqa
            {
                Id = Guid.NewGuid(),
                FoujId = request.FoujId,
                Name = request.Name,
                TeacherId = request.TeacherId,
                Type = request.Type
            };

            await _unitOfWork.Repository<Halqa>().AddAsync(halqa);

            var assignmentResult = await SyncStudentsAsync(halqa.Id, request.StudentIds, managedMosqueId, isCreate: true);
            if (assignmentResult != null)
                return assignmentResult;

            await _unitOfWork.CompleteAsync();

            var assignedCount = await CountAssignedStudentsAsync(halqa.Id);

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj.name,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = teacher.name,
                Type = halqa.Type,
                StudentsCount = assignedCount
            };

            return GeneralResponse.Ok("تم إنشاء الحلقة بنجاح.", dto);
        }

        public async Task<GeneralResponse> UpdateHalqaAsync(UpdateHalqaRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue && halqa.Fouj.MosqueId != managedMosqueId.Value)
                return GeneralResponse.BadRequest("لا يمكنك تعديل هذه الحلقة.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                halqa.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Type))
                halqa.Type = request.Type;

            Fouj? resultFouj = halqa.Fouj;
            Teacher? resultTeacher = null;

            if (request.FoujId.HasValue)
            {
                resultFouj = await _context.Foujs.FirstOrDefaultAsync(f => f.Id == request.FoujId.Value);
                if (resultFouj == null)
                    return GeneralResponse.BadRequest("الفوج المستهدف غير موجود.");

                if (managedMosqueId.HasValue && resultFouj.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك نقل الحلقة إلى فوج خارج نطاق المسجد الذي تديره.");

                halqa.FoujId = request.FoujId.Value;
            }

            if (request.TeacherId.HasValue)
            {
                resultTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == request.TeacherId.Value);
                if (resultTeacher == null)
                    return GeneralResponse.BadRequest("المعلم غير موجود.");

                if (resultTeacher.status != 0)
                    return GeneralResponse.BadRequest("لا يمكن تعيين معلم غير نشط على حلقة.");

                if (managedMosqueId.HasValue && resultTeacher.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك تعيين معلم خارج نطاق المسجد الذي تديره.");

                halqa.TeacherId = request.TeacherId.Value;
            }

            var assignmentResult = await SyncStudentsAsync(halqa.Id, request.StudentIds, managedMosqueId, isCreate: false);
            if (assignmentResult != null)
                return assignmentResult;

            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            resultFouj ??= await _context.Foujs.FirstOrDefaultAsync(f => f.Id == halqa.FoujId);
            resultTeacher ??= halqa.TeacherId.HasValue
                ? await _context.Teachers.FirstOrDefaultAsync(t => t.Id == halqa.TeacherId.Value)
                : null;

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = resultFouj?.name ?? string.Empty,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = resultTeacher?.name ?? string.Empty,
                Type = halqa.Type,
                StudentsCount = await CountAssignedStudentsAsync(halqa.Id)
            };

            return GeneralResponse.Ok("تم تحديث الحلقة بنجاح.", dto);
        }

        public async Task<GeneralResponse> DeleteHalqaAsync(DeleteHalqaRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue && halqa.Fouj.MosqueId != managedMosqueId.Value)
                return GeneralResponse.BadRequest("لا يمكنك حذف هذه الحلقة.");

            var blockers = new List<string>();

            var studentsCount = await _context.Students.CountAsync(s => s.role == StudentRole && s.HalqaId == halqa.Id);
            if (studentsCount > 0)
                blockers.Add($"{studentsCount} طالب");

            var progressEntriesCount = await _context.ProgressEntries.CountAsync(p => p.HalqaId == halqa.Id);
            if (progressEntriesCount > 0)
                blockers.Add($"{progressEntriesCount} سجل تقدم");

            var sessionsCount = await _context.HalqaSessions.CountAsync(s => s.HalqaId == halqa.Id);
            if (sessionsCount > 0)
                blockers.Add($"{sessionsCount} جلسة حلقة");

            var examAssignmentsCount = await _context.ExamTeacherHalqa.CountAsync(x => x.HalqaId == halqa.Id);
            if (examAssignmentsCount > 0)
                blockers.Add($"{examAssignmentsCount} تكليف اختبار");

            var postsCount = await _context.Posts.CountAsync(p => p.HalqaId == halqa.Id);
            if (postsCount > 0)
                blockers.Add($"{postsCount} منشور");

            var weeklyLessonsCount = await _context.Set<SaturdayLesson>().CountAsync(l => l.HalqaId == halqa.Id);
            if (weeklyLessonsCount > 0)
                blockers.Add($"{weeklyLessonsCount} موعد درس أسبوعي");

            if (blockers.Count > 0)
            {
                var blockerText = string.Join("، ", blockers);
                return GeneralResponse.BadRequest($"لا يمكن حذف الحلقة لأنها مرتبطة ببيانات أخرى: {blockerText}. يرجى حذف أو نقل البيانات المرتبطة أولاً.");
            }

            await _unitOfWork.Repository<Halqa>().DeleteAsync(halqa);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف الحلقة بنجاح.");
        }

        public async Task<GeneralResponse> MoveToFoujAsync(MoveHalqaToFoujRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var targetFouj = await _context.Foujs.FirstOrDefaultAsync(f => f.Id == request.TargetFoujId);
            if (targetFouj == null)
                return GeneralResponse.BadRequest("الفوج المستهدف غير موجود.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue)
            {
                if (halqa.Fouj.MosqueId != managedMosqueId.Value || targetFouj.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك نقل الحلقة خارج نطاق المسجد الذي تديره.");
            }

            halqa.FoujId = request.TargetFoujId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var teacher = halqa.TeacherId.HasValue
                ? await _context.Teachers.FirstOrDefaultAsync(t => t.Id == halqa.TeacherId.Value)
                : null;

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = targetFouj.name,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = teacher?.name ?? string.Empty,
                Type = halqa.Type,
                StudentsCount = await CountAssignedStudentsAsync(halqa.Id)
            };

            return GeneralResponse.Ok("تم نقل الحلقة إلى الفوج بنجاح.", dto);
        }

        public async Task<GeneralResponse> ReassignTeacherAsync(ReassignHalqaTeacherRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("الحلقة غير موجودة.");

            var newTeacher = await _context.Teachers.FirstOrDefaultAsync(t => t.Id == request.NewTeacherId);
            if (newTeacher == null)
                return GeneralResponse.BadRequest("المعلم الجديد غير موجود.");

            if (newTeacher.status != 0)
                return GeneralResponse.BadRequest("لا يمكن تعيين معلم غير نشط على حلقة.");

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue)
            {
                if (halqa.Fouj.MosqueId != managedMosqueId.Value || newTeacher.MosqueId != managedMosqueId.Value)
                    return GeneralResponse.BadRequest("لا يمكنك إعادة تعيين معلم خارج نطاق المسجد الذي تديره.");
            }

            halqa.TeacherId = request.NewTeacherId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var dto = new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = halqa.Fouj?.name ?? string.Empty,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = newTeacher.name,
                Type = halqa.Type,
                StudentsCount = await CountAssignedStudentsAsync(halqa.Id)
            };

            return GeneralResponse.Ok("تم إعادة تعيين المعلم للحلقة بنجاح.", dto);
        }

        private async Task<GeneralResponse?> SyncStudentsAsync(Guid halqaId, List<Guid>? studentIds, Guid? managedMosqueId, bool isCreate)
        {
            if (studentIds == null)
                return null;

            var requestedIds = studentIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var requestedStudents = requestedIds.Count == 0
                ? new List<Student>()
                : await _context.Students
                    .Where(s => requestedIds.Contains(s.Id))
                    .ToListAsync();

            if (requestedStudents.Count != requestedIds.Count)
                return GeneralResponse.BadRequest("يوجد طالب واحد أو أكثر غير موجود.");

            if (requestedStudents.Any(s => s.role != StudentRole))
                return GeneralResponse.BadRequest("يمكن ربط الطلاب فقط بالحلقة.");

            if (managedMosqueId.HasValue && requestedStudents.Any(s => s.MosqueId != managedMosqueId.Value))
                return GeneralResponse.BadRequest("لا يمكنك إدارة طلاب خارج نطاق المسجد الذي تديره.");

            var blockedStudent = requestedStudents.FirstOrDefault(s =>
                s.HalqaId.HasValue &&
                s.HalqaId != Guid.Empty &&
                s.HalqaId != halqaId);

            if (blockedStudent != null)
            {
                return GeneralResponse.BadRequest($"الطالب {blockedStudent.name} مرتبط بحلقة أخرى بالفعل.");
            }

            if (!isCreate)
            {
                var currentStudents = await _context.Students
                    .Where(s => s.role == StudentRole && s.HalqaId == halqaId)
                    .ToListAsync();

                foreach (var student in currentStudents.Where(s => !requestedIds.Contains(s.Id)))
                    student.HalqaId = null;
            }

            foreach (var student in requestedStudents)
                student.HalqaId = halqaId;

            return null;
        }

        private async Task<Guid?> ResolveManagedMosqueIdAsync()
        {
            var currentUserId = _currentUserService.CurrentUserId;
            if (!currentUserId.HasValue)
                return null;

            return await _context.Supervisors
                .Where(s => s.Id == currentUserId.Value)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync();
        }

        private Task<int> CountAssignedStudentsAsync(Guid halqaId)
            => _context.Students.CountAsync(s => s.role == StudentRole && s.HalqaId == halqaId);
    }
}
