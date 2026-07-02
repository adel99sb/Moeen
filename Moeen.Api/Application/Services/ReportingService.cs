using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Reporting;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Goal;
using Moeen.Shared.Responses.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudentProgressReportDto = Moeen.Shared.Responses.Reporting.StudentProgressReportDto;

namespace Moeen.Api.Application.Services
{
    public class ReportingService : IReportingService
    {
        private readonly AppDbContext _context;

        public ReportingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetGeneralPerformanceIndicatorsAsync(GetGeneralPerformanceIndicatorsRequest request)
        {
            var totalStudents = await _context.Students.AsNoTracking().CountAsync(s => s.role == 2 && s.status == 0);
            var totalTeachers = await _context.Teachers.AsNoTracking().CountAsync(t => t.status == 0);
            var totalCircles = await _context.Halqas.AsNoTracking().CountAsync();
            var totalComplaints = await _context.Complaints.AsNoTracking().CountAsync();

            var topStudents = await _context.Students.AsNoTracking()
                .Where(s => s.role == 2 && s.status == 0)
                .OrderByDescending(s => s.score)
                .Take(5)
                .Select(s => new StudentScoreDto
                {
                    StudentId = s.Id,
                    StudentName = s.name ?? string.Empty,
                    Score = s.score
                })
                .ToListAsync();

            var lowStudents = await _context.Students.AsNoTracking()
                .Where(s => s.role == 2 && s.status == 0)
                .OrderBy(s => s.score)
                .Take(5)
                .Select(s => new StudentScoreDto
                {
                    StudentId = s.Id,
                    StudentName = s.name ?? string.Empty,
                    Score = s.score
                })
                .ToListAsync();

            var data = new GeneralPerformanceIndicatorsDto
            {
                TotalStudents = totalStudents,
                TotalTeachers = totalTeachers,
                TotalCircles = totalCircles,
                TotalComplaints = totalComplaints,
                TopStudents = topStudents,
                LowStudents = lowStudents
            };

            return GeneralResponse.Ok("تم جلب مؤشرات الأداء العام.", data);
        }

        public async Task<GeneralResponse> GetMonthlyCirclePerformanceAsync(GetMonthlyCirclePerformanceRequest request)
        {
            request ??= new GetMonthlyCirclePerformanceRequest();

            var fromDate = request.FromDate?.Date ?? new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);
            var toDate = request.ToDate?.Date ?? fromDate.AddMonths(1).AddDays(-1);

            if (fromDate > toDate)
                return GeneralResponse.BadRequest("نطاق التاريخ غير صالح.");

            var circlesQuery = _context.Halqas.AsNoTracking();

            if (request.TeacherId.HasValue && request.TeacherId.Value != Guid.Empty)
                circlesQuery = circlesQuery.Where(h => h.TeacherId == request.TeacherId.Value);

            var circles = await circlesQuery.ToListAsync();
            var circleIds = circles.Select(c => c.Id).ToList();

            if (circleIds.Count == 0)
                return GeneralResponse.Ok("لا توجد حلقات مطابقة.", new List<CircleMonthlyPerformanceDto>());

            var attendanceDict = await _context.Attendances
                .Join(_context.HalqaSessions, a => a.HalqeSessionId, s => s.Id, (a, s) => new { s.HalqaId, s.date })
                .Where(x => circleIds.Contains(x.HalqaId) && x.date >= fromDate && x.date <= toDate)
                .GroupBy(x => x.HalqaId)
                .Select(g => new { HalqaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.HalqaId, x => x.Count);

            var progressQuery = _context.ProgressEntries.AsNoTracking()
                .Where(p => circleIds.Contains(p.HalqaId) && !p.IsDeleted && p.Student.status == 0 && p.Date >= fromDate && p.Date <= toDate);

            var memorizationDict = await progressQuery
                .Where(p => p.NextTarget > 0)
                .GroupBy(p => p.HalqaId)
                .Select(g => new { HalqaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.HalqaId, x => x.Count);

            var reviewDict = await progressQuery
                .Where(p => p.NextTarget == 0)
                .GroupBy(p => p.HalqaId)
                .Select(g => new { HalqaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.HalqaId, x => x.Count);

            var examDict = await _context.Exams
                .Join(_context.Students, e => e.StudentId, s => s.Id, (e, s) => new { e, s.HalqaId, StudentStatus = s.status })
                .Where(x => x.StudentStatus == 0 && x.HalqaId.HasValue && circleIds.Contains(x.HalqaId.Value) && x.e.date >= fromDate && x.e.date <= toDate)
                .GroupBy(x => x.HalqaId.Value)
                .Select(g => new { HalqaId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.HalqaId, x => x.Count);

            var data = circles.Select(c => new CircleMonthlyPerformanceDto
            {
                CircleId = c.Id,
                CircleName = c.Name ?? string.Empty,
                AttendanceCount = attendanceDict.GetValueOrDefault(c.Id, 0),
                MemorizationCount = memorizationDict.GetValueOrDefault(c.Id, 0),
                ReviewCount = reviewDict.GetValueOrDefault(c.Id, 0),
                ExamCount = examDict.GetValueOrDefault(c.Id, 0)
            }).ToList();

            return GeneralResponse.Ok("تم جلب أداء الحلقات الشهري.", data);
        }

        public async Task<GeneralResponse> GetStudentProgressTimelineAsync(GetStudentProgressTimelineRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف الطالب مطلوب.");

            var student = await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == request.StudentId && s.status == 0);

            if (student == null)
                return GeneralResponse.NotFound("الطالب غير موجود.");

            var fromDate = request.FromDate?.Date ?? DateTime.UtcNow.AddDays(-30).Date;
            var toDate = request.ToDate?.Date ?? DateTime.UtcNow.Date;

            if (fromDate > toDate)
                return GeneralResponse.BadRequest("نطاق التاريخ غير صالح.");

            var progressQuery = _context.ProgressEntries.AsNoTracking()
                .Where(p => p.StudentId == request.StudentId && !p.IsDeleted && p.Date >= fromDate && p.Date <= toDate);

            var memorizationEntries = await progressQuery.Where(p => p.NextTarget > 0).ToListAsync();
            var reviewEntries = await progressQuery.Where(p => p.NextTarget == 0).ToListAsync();

            var exams = await _context.Exams.AsNoTracking()
                .Where(e => e.StudentId == request.StudentId && e.date >= fromDate && e.date <= toDate)
                .ToListAsync();

            var attendanceCount = await _context.Attendances
                .Join(_context.HalqaSessions, a => a.HalqeSessionId, s => s.Id, (a, s) => new { a.StudentId, s.date })
                .Where(x => x.StudentId == request.StudentId && x.date >= fromDate && x.date <= toDate)
                .CountAsync();

            var records = new List<Moeen.Shared.Responses.Reporting.StudentProgressRecordDto>();

            records.AddRange(memorizationEntries.Select(e => new Moeen.Shared.Responses.Reporting.StudentProgressRecordDto
            {
                RecordId = e.Id,
                Type = "Memorization",
                Date = e.Date,
                JuzFrom = e.JuzNumber,
                JuzTo = e.JuzNumber,
                PageFrom = e.PageNumber,
                PageTo = e.MemorizedUntil,
                Points = e.LevelScore
            }));

            records.AddRange(reviewEntries.Select(e => new Moeen.Shared.Responses.Reporting.StudentProgressRecordDto
            {
                RecordId = e.Id,
                Type = "Review",
                Date = e.Date,
                JuzFrom = e.JuzNumber,
                JuzTo = e.JuzNumber,
                PageFrom = e.PageNumber,
                PageTo = e.MemorizedUntil,
                Points = e.LevelScore
            }));
            
            records.AddRange(exams.Select(e => new Moeen.Shared.Responses.Reporting.StudentProgressRecordDto
            {
                RecordId = e.Id,
                Type = "Exam",
                Date = e.date,
                JuzFrom = e.juz_form,
                JuzTo = e.juz_to,
                Score = e.score,
                Notes = e.notes
            }));

            if (!string.IsNullOrWhiteSpace(request.Type))
                records = records.Where(r => r.Type.Equals(request.Type, StringComparison.OrdinalIgnoreCase)).ToList();

            records = records.OrderByDescending(r => r.Date).ToList();

            var totalCount = records.Count;
            var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
            var pageSize = request.PageSize > 0 ? request.PageSize : 20;

            var pageRecords = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var summary = new Moeen.Shared.Responses.Reporting.StudentProgressSummaryDto
            {
                TotalPoints = student.score,
                ExamsCount = exams.Count,
                MemorizationCount = memorizationEntries.Count,
                ReviewCount = reviewEntries.Count,
                AttendanceCount = attendanceCount
            };

            var data = new StudentProgressReportDto
            {
                Summary = summary,
                Records = pageRecords
            };

            return GeneralResponse.Ok("تم جلب سجل التقدم.", data, pageNumber, pageSize, totalCount);
        }
    }
}
