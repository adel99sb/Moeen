using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Points;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Points;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class PointsService : IPointsService
    {
        private const int AutomaticBonusPoints = 10;

        private readonly AppDbContext _context;

        public PointsService(AppDbContext context)
        {
            _context = context;
        }

        public Task<GeneralResponse> SetupPointsSystemAsync(SetupPointsSystemRequest request)
        {
            if (request?.PointsMap == null || request.PointsMap.Count == 0)
                return Task.FromResult(GeneralResponse.BadRequest("لا توجد بيانات لإعداد نظام النقاط."));

            var normalizedMap = request.PointsMap
                .GroupBy(x => x.Grade)
                .Select(g => g.Last())
                .OrderBy(x => x.Grade)
                .ToList();

            return Task.FromResult(GeneralResponse.Ok("تم استلام إعدادات النقاط.", normalizedMap));
        }

        public async Task<GeneralResponse> GetStudentPointsAsync(GetStudentPointsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.StudentId);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            return GeneralResponse.Ok("تم جلب رصيد الطالب.", new GetStudentPointsResponse
            {
                Points = student.score
            });
        }

        public async Task<GeneralResponse> GetPointsLeaderboardAsync(GetLeaderboardRequest request)
        {
            request ??= new GetLeaderboardRequest();

            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var query = _context.Students.AsNoTracking();

            if (request.CircleId.HasValue && request.CircleId.Value != Guid.Empty)
                query = query.Where(s => s.SaturdayHalqeId == request.CircleId.Value);

            var totalCount = await query.CountAsync();
            var skip = (pageNumber - 1) * pageSize;

            var students = await query
                .OrderByDescending(s => s.score)
                .ThenBy(s => s.name)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();

            var leaderboard = students
                .Select((student, index) => new StudentLeaderboardDto
                {
                    StudentId = student.Id,
                    StudentName = student.name ?? string.Empty,
                    TotalPoints = student.score,
                    Rank = skip + index + 1
                })
                .ToList();

            return GeneralResponse.Ok("تم جلب لوحة الصدارة.", leaderboard, pageNumber, pageSize, totalCount);
        }

        public async Task<GeneralResponse> AwardPointsManuallyAsync(AwardPointsManualRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            if (string.IsNullOrWhiteSpace(request.PointTypeKey))
                return GeneralResponse.BadRequest("نوع النقاط مطلوب.");

            if (request.Points <= 0)
                return GeneralResponse.BadRequest("قيمة النقاط غير صحيحة.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            student.score += request.Points;
            if (student.score < 0)
                student.score = 0;

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم منح النقاط بنجاح.", new GetStudentPointsResponse
            {
                Points = student.score
            });
        }

        public async Task<GeneralResponse> RemovePointsManuallyAsync(RemovePointsManualRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            if (request.Points <= 0)
                return GeneralResponse.BadRequest("قيمة النقاط غير صحيحة.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            student.score -= request.Points;
            if (student.score < 0)
                student.score = 0;

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم خصم النقاط بنجاح.", new GetStudentPointsResponse
            {
                Points = student.score
            });
        }

        public async Task<GeneralResponse> EvaluateAutomaticPointsAsync(EvaluateAutomaticPointsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students.FirstOrDefaultAsync(s => s.Id == request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var endDate = (request.ToDate ?? DateTime.UtcNow.Date).Date;
            var startDate = (request.FromDate ?? endDate.AddDays(-6)).Date;

            if (startDate > endDate)
                return GeneralResponse.BadRequest("نطاق التاريخ غير صالح.");

            if ((endDate - startDate).TotalDays < 6)
                return GeneralResponse.BadRequest("يجب أن يغطي التقييم أسبوعاً كاملاً.");

            var bonusDetails = new
            {
                AttendanceBonus = 0,
                ReviewBonus = 0,
                MemorizationBonus = 0
            };

            var attendanceBonus = await HasFullAttendanceWeekAsync(student.Id, startDate, endDate) ? AutomaticBonusPoints : 0;
            var reviewBonus = await HasFiveExcellentReviewJuzAsync(student.Id, startDate, endDate) ? AutomaticBonusPoints : 0;
            var memorizationBonus = await HasFiveNewMemorizedPagesAsync(student.Id, startDate, endDate) ? AutomaticBonusPoints : 0;

            var addedPoints = attendanceBonus + reviewBonus + memorizationBonus;

            if (addedPoints > 0)
            {
                student.score += addedPoints;
                if (student.score < 0)
                    student.score = 0;

                await _context.SaveChangesAsync();
            }

            return GeneralResponse.Ok(
                addedPoints == 0 ? "لا توجد مكافآت تلقائية مستحقة." : "تمت إضافة النقاط التلقائية بنجاح.",
                new
                {
                    bonusDetails.AttendanceBonus,
                    bonusDetails.ReviewBonus,
                    bonusDetails.MemorizationBonus,
                    AddedPoints = addedPoints,
                    CurrentPoints = student.score
                });
        }

        private async Task<bool> HasFullAttendanceWeekAsync(Guid studentId, DateTime startDate, DateTime endDate)
        {
            var totalDays = (endDate.Date - startDate.Date).Days + 1;

            var attendedDays = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.HalqeSession)
                .Where(a =>
                    a.StudentId == studentId &&
                    a.HalqeSession.date.Date >= startDate.Date &&
                    a.HalqeSession.date.Date <= endDate.Date &&
                    (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late))
                .Select(a => a.HalqeSession.date.Date)
                .Distinct()
                .CountAsync();

            return totalDays >= 7 && attendedDays >= 7;
        }

        private async Task<bool> HasFiveExcellentReviewJuzAsync(Guid studentId, DateTime startDate, DateTime endDate)
        {
            var excellentJuzCount = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p =>
                    p.StudentId == studentId &&
                    p.Date.Date >= startDate.Date &&
                    p.Date.Date <= endDate.Date &&
                    p.LevelScore >= 5)
                .Select(p => p.JuzNumber)
                .Distinct()
                .CountAsync();

            return excellentJuzCount >= 5;
        }

        private async Task<bool> HasFiveNewMemorizedPagesAsync(Guid studentId, DateTime startDate, DateTime endDate)
        {
            var pageCount = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p =>
                    p.StudentId == studentId &&
                    p.Date.Date >= startDate.Date &&
                    p.Date.Date <= endDate.Date)
                .Select(p => p.PageNumber)
                .Distinct()
                .CountAsync();

            return pageCount >= 5;
        }
    }
}