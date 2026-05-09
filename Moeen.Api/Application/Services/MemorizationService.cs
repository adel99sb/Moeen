using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Memorization;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Memorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class MemorizationService : IMemorizationService
    {
        private const int QuranPages = 604;
        private const int PagesPerJuz = 20;

        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public MemorizationService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> RecordNewPageMemorizationAsync(RecordPageMemorizationRequest request)
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
                studentId = request.StudentId,
                TeacherId = teacherId.Value,
                HalqaId = halqaId,
                date = DateTime.UtcNow,
                page_number = request.PageNumber,
                memorized_until = request.PageNumber,
                next_target = request.PageNumber + 1,
                juz_number = CalculateJuzNumber(request.PageNumber),
                level_score = points
            };

            await _context.ProgressEntries.AddAsync(entry);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تسجيل حفظ الصفحة.", new RecordPageMemorizationResponse
            {
                PointsEarned = points
            });
        }

        public async Task<GeneralResponse> GetLastMemorizedPageAsync(GetLastMemorizedPageRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var lastPage = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.studentId == request.StudentId)
                .MaxAsync(p => (int?)p.page_number) ?? 0;

            return GeneralResponse.Ok("تم جلب آخر صفحة محفوظة.", new GetLastMemorizedPageResponse
            {
                PageNumber = lastPage
            });
        }

        public async Task<GeneralResponse> GetMemorizationRecordAsync(GetMemorizationRecordRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var record = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.studentId == request.StudentId && p.page_number == request.PageNumber)
                .OrderByDescending(p => p.date)
                .FirstOrDefaultAsync();

            if (record == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            return GeneralResponse.Ok("تم جلب السجل.", MapRecord(record));
        }

        public async Task<GeneralResponse> GetStudentMemorizationHistoryAsync(GetStudentMemorizationHistoryRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var query = _context.ProgressEntries.AsNoTracking()
                .Where(p => p.studentId == request.StudentId && p.date >= fromDate && p.date <= toDate)
                .OrderByDescending(p => p.date);

            var totalCount = await query.CountAsync();
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب سجل الحفظ.", items.Select(MapRecord).ToList(), pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetMemorizationStatisticsAsync(GetMemorizationStatisticsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-90)).Date;
            var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

            var entries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.studentId == request.StudentId && p.date >= fromDate && p.date <= toDate)
                .ToListAsync();

            var totalPages = entries.Select(e => e.page_number).Distinct().Count();
            var maxPage = entries.Count == 0 ? 0 : entries.Max(e => e.page_number);
            var totalJuz = maxPage / PagesPerJuz;

            var weeks = Math.Max(1, (toDate - fromDate).TotalDays / 7d);
            var avgPerWeek = totalPages / weeks;

            var masteryRate = entries.Count == 0
                ? 0
                : (entries.Average(e => e.level_score) / 5d) * 100d;

            return GeneralResponse.Ok("تم جلب الإحصائيات.", new MemorizationStatisticsDto
            {
                TotalPagesMemorized = totalPages,
                TotalJuzCompleted = totalJuz,
                AveragePagesPerWeek = Math.Round(avgPerWeek, 2),
                MasteryRate = Math.Round(masteryRate, 2)
            });
        }

        public async Task<GeneralResponse> GetCircleMemorizationProgressAsync(GetCircleProgressRequest request)
        {
            if (request == null || request.CircleId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الحلقة مطلوب.");

            var studentIds = await _context.Students
                .Where(s => s.SaturdayHalqeId == request.CircleId)
                .Select(s => s.Id)
                .ToListAsync();

            var entries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => studentIds.Contains(p.studentId))
                .ToListAsync();

            var grouped = entries
                .GroupBy(e => e.studentId)
                .ToDictionary(g => g.Key, g => new
                {
                    LastPage = g.Max(x => x.page_number),
                    TotalPages = g.Select(x => x.page_number).Distinct().Count()
                });

            var students = await _context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync();

            var result = students.Select(s =>
            {
                grouped.TryGetValue(s.Id, out var data);
                var lastPage = data?.LastPage ?? 0;
                var totalPages = data?.TotalPages ?? 0;

                return new StudentMemorizationSummaryDto
                {
                    StudentId = s.Id,
                    StudentName = s.name,
                    LastMemorizedPage = lastPage,
                    TotalPagesMemorized = totalPages,
                    ProgressPercentage = Math.Round((double)lastPage / QuranPages * 100d, 2)
                };
            }).ToList();

            return GeneralResponse.Ok("تم جلب تقدم الحفظ للحلقة.", result);
        }

        public async Task<GeneralResponse> GetMemorizationProgressReportAsync(GetProgressReportRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var entries = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.studentId == request.StudentId && p.date >= request.FromDate && p.date <= request.ToDate)
                .OrderBy(p => p.date)
                .ToListAsync();

            var grouped = entries
                .GroupBy(e => e.date.Date)
                .OrderBy(g => g.Key)
                .Select(g => new { Date = g.Key, Count = g.Count() })
                .ToList();

            var cumulative = 0;
            var points = grouped.Select(g =>
            {
                cumulative += g.Count;
                return new MemorizationProgressPointDto
                {
                    Date = g.Date,
                    PagesMemorizedCumulative = cumulative
                };
            }).ToList();

            return GeneralResponse.Ok("تم جلب التقرير.", new MemorizationProgressReportDto
            {
                StudentId = request.StudentId,
                Points = points
            });
        }

        public async Task<GeneralResponse> UpdateMemorizationGradeAsync(UpdateMemorizationGradeRequest request)
        {
            if (request == null || request.RecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            var entry = await _context.ProgressEntries.FindAsync(request.RecordId);
            if (entry == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            entry.level_score = MapGradeToPoints(request.Grade);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تحديث التقدير.", MapRecord(entry));
        }

        public async Task<GeneralResponse> DeleteMemorizationRecordAsync(DeleteMemorizationRecordRequest request)
        {
            if (request == null || request.RecordId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف السجل مطلوب.");

            var entry = await _context.ProgressEntries.FindAsync(request.RecordId);
            if (entry == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            _context.ProgressEntries.Remove(entry);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف السجل.");
        }

        private async Task<Guid> ResolveHalqaIdAsync(Guid teacherId, Guid? fallbackHalqaId)
        {
            var halqaId = await _context.Halqas
                .Where(h => h.TeacherId == teacherId)
                .Select(h => h.Id)
                .FirstOrDefaultAsync();

            return halqaId != Guid.Empty ? halqaId : (fallbackHalqaId ?? Guid.Empty);
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

        private static MemorizationRecordDto MapRecord(ProgressEntry entry)
        {
            return new MemorizationRecordDto
            {
                RecordId = entry.Id,
                StudentId = entry.studentId,
                PageNumber = entry.page_number,
                Grade = ResolveGrade(entry.level_score),
                Notes = null,
                MemorizedAt = entry.date
            };
        }
    }
}