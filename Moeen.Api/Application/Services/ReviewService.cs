using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Review;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Review;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class ReviewService : IReviewService
    {
        private const int PagesPerJuz = 20;

        private readonly AppDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public ReviewService(AppDbContext context, IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> RecordReviewPageAsync(RecordReviewPageRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var halqaId = await ResolveHalqaIdAsync(teacherId.Value, student.SaturdayHalqeId);
            if (halqaId == Guid.Empty)
                return GeneralResponse.BadRequest("لا توجد حلقة مرتبطة.");

            var points = MapGradeToPoints(request.Grade);

            var entry = new ProgressEntry
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                TeacherId = teacherId.Value,
                HalqaId = halqaId,
                JuzNumber = CalculateJuzNumber(request.PageNumber),
                PageNumber = request.PageNumber,
                MemorizedUntil = request.PageNumber,
                NextTarget = 0, // تمييز المراجعة عن التسميع
                LevelScore = points,
                Date = DateTime.UtcNow,
                IsDeleted = false,
                DeletedAt = null
            };

            await _unitOfWork.Repository<ProgressEntry>().AddAsync(entry);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تسجيل المراجعة اليومية.", new RecordReviewPageResponse
            {
                PointsEarned = points
            });
        }

        public async Task<GeneralResponse> RecordJuzReviewAsync(RecordJuzReviewRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var halqaId = await ResolveHalqaIdAsync(teacherId.Value, student.SaturdayHalqeId);
            if (halqaId == Guid.Empty)
                return GeneralResponse.BadRequest("لا توجد حلقة مرتبطة.");

            var points = MapGradeToPoints(request.OverallGrade);

            var startPage = ((request.JuzNumber - 1) * PagesPerJuz) + 1;
            var endPage = request.JuzNumber * PagesPerJuz;

            var entry = new ProgressEntry
            {
                Id = Guid.NewGuid(),
                StudentId = request.StudentId,
                TeacherId = teacherId.Value,
                HalqaId = halqaId,
                JuzNumber = request.JuzNumber,
                PageNumber = startPage,
                MemorizedUntil = endPage,
                NextTarget = 0, // تمييز المراجعة عن التسميع
                LevelScore = points,
                Date = DateTime.UtcNow,
                IsDeleted = false,
                DeletedAt = null
            };

            await _unitOfWork.Repository<ProgressEntry>().AddAsync(entry);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تسجيل مراجعة الجزء.", new ReviewResultDto
            {
                JuzNumber = request.JuzNumber,
                Success = true,
                PointsEarned = points,
                Notes = request.Notes,
                ErrorMessage = string.Empty
            });
        }

        public async Task<GeneralResponse> GetReviewRecordByIdAsync(GetReviewRecordByIdRequest request)
        {
            if (request == null || request.ReviewRecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            var record = await _context.ProgressEntries
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == request.ReviewRecordId && r.NextTarget == 0 && !r.IsDeleted);

            if (record == null)
                return GeneralResponse.NotFound("سجل المراجعة غير موجود.");

            return GeneralResponse.Ok("تم جلب السجل.", MapReviewRecord(record));
        }

        public async Task<GeneralResponse> GetStudentReviewHistoryAsync(GetStudentReviewHistoryRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            if (request.ReviewType.HasValue && request.ReviewType.Value != ReviewType.Daily)
                return GeneralResponse.Ok("لا توجد مراجعات مطابقة.", new PagedList<ReviewRecordDto>
                {
                    Items = [], 
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize,
                    TotalCount = 0
                }, request.PageNumber, request.PageSize, 0);

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var query = _context.ProgressEntries.AsNoTracking()
                .Where(r => r.StudentId == request.StudentId &&
                            r.NextTarget == 0 &&
                            !r.IsDeleted &&
                            r.Date >= fromDate &&
                            r.Date <= toDate)
                .OrderByDescending(r => r.Date);

            var totalCount = await query.CountAsync();
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var records = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new PagedList<ReviewRecordDto>
            {
                Items = records.Select(MapReviewRecord).ToList(),
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount
            };

            return GeneralResponse.Ok("تم جلب سجل المراجعات.", result, pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> UpdateReviewGradeAsync(UpdateReviewGradeRequest request)
        {
            if (request == null || request.ReviewRecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var record = await _context.ProgressEntries
                .FirstOrDefaultAsync(r => r.Id == request.ReviewRecordId && r.NextTarget == 0 && !r.IsDeleted);

            if (record == null)
                return GeneralResponse.NotFound("سجل المراجعة غير موجود.");

            if (!await IsTeacherAssignedToStudentAsync(teacherId.Value, record.StudentId))
                return GeneralResponse.Unauthorized("غير مصرح.");

            record.LevelScore = MapGradeToPoints(request.NewGrade);

            await _unitOfWork.Repository<ProgressEntry>().UpdateAsync(record);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم تحديث التقدير.", MapReviewRecord(record));
        }

        public async Task<GeneralResponse> DeleteReviewRecordAsync(DeleteReviewRecordRequest request)
        {
            if (request == null || request.ReviewRecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var record = await _context.ProgressEntries
                .FirstOrDefaultAsync(r => r.Id == request.ReviewRecordId && r.NextTarget == 0 && !r.IsDeleted);

            if (record == null)
                return GeneralResponse.NotFound("سجل المراجعة غير موجود.");

            if (!await IsTeacherAssignedToStudentAsync(teacherId.Value, record.StudentId))
                return GeneralResponse.Unauthorized("غير مصرح.");

            record.IsDeleted = true;
            record.DeletedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<ProgressEntry>().UpdateAsync(record);
            await _unitOfWork.CompleteAsync();

            return GeneralResponse.Ok("تم حذف سجل المراجعة.");
        }

        private async Task<Guid> ResolveHalqaIdAsync(Guid teacherId, Guid fallbackHalqaId)
        {
            var halqaId = await _context.Halqas
                .Where(h => h.TeacherId == teacherId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            return halqaId != Guid.Empty ? halqaId : fallbackHalqaId;
        }

        private async Task<bool> IsTeacherAssignedToStudentAsync(Guid teacherId, Guid studentId)
        {
            var student = await _context.Students
                .AsNoTracking()
                .Where(s => s.Id == studentId)
                .Select(s => new { s.HalqaId, s.SaturdayHalqaId, s.SaturdayHalqeId })
                .FirstOrDefaultAsync();

            if (student == null)
                return false;

            if (student.HalqaId.HasValue && student.HalqaId.Value != Guid.Empty)
            {
                var hasHalqaAccess = await _context.Halqas
                    .AnyAsync(h => h.Id == student.HalqaId && h.TeacherId == teacherId);

                if (hasHalqaAccess)
                    return true;
            }

            var saturdayHalqaId = student.SaturdayHalqaId ?? student.SaturdayHalqeId;
            if (saturdayHalqaId != Guid.Empty)
            {
                return await _context.SaturdayHalqes
                    .AnyAsync(h => h.Id == saturdayHalqaId && h.TeacherId == teacherId);
            }

            return false;
        }

        private static int CalculateJuzNumber(int pageNumber)
            => Math.Clamp(((pageNumber - 1) / PagesPerJuz) + 1, 1, 30);

        private static int MapGradeToPoints(Grade grade) =>
            grade switch
            {
                Grade.Excellent => 5,
                Grade.VeryGood => 4,
                Grade.Good => 3,
                Grade.Acceptable => 2,
                _ => 1
            };

        private static Grade ResolveGrade(int points) =>
            points switch
            {
                5 => Grade.Excellent,
                4 => Grade.VeryGood,
                3 => Grade.Good,
                2 => Grade.Acceptable,
                _ => Grade.Weak
            };

        private static ReviewRecordDto MapReviewRecord(ProgressEntry entry)
        {
            return new ReviewRecordDto
            {
                ReviewRecordId = entry.Id,
                StudentId = entry.StudentId,
                PageNumber = entry.PageNumber,
                JuzNumber = entry.JuzNumber,
                ReviewType = ReviewType.Daily,
                Grade = ResolveGrade(entry.LevelScore),
                Notes = null,
                ReviewedAt = entry.Date
            };
        }
    }
}