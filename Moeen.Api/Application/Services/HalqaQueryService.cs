using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Responses.Halqa;
using Moeen.Shared.Responses.HalqaQuery;
using Moeen.Shared.Responses.Enrollment;

namespace Moeen.Api.Application.Services
{
    public class HalqaQueryService : IHalqaQueryService
    {
        private readonly AppDbContext _context;

        public HalqaQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<HalqaDto> GetHalqaByIdAsync(GetHalqaByIdRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .Include(h => h.Teacher)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.HalqaId));

            // Students count computed in DB (using ProgressEntry relation)
            var studentsCount = await _context.ProgressEntries
                .Where(pe => pe.HalqaId == request.CircleId)
                .Select(pe => pe.StudentId)
                .Distinct()
                .CountAsync();

            return new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = halqa.Fouj?.name,
                TeacherId = (Guid)halqa.TeacherId,
                TeacherName = halqa.Teacher?.name,
                Type = halqa.Type,
                StudentsCount = studentsCount
            };
        }

        public async Task<HalqaStudentsResponse> GetHalqaStudentsAsync(GetHalqaStudentsRequest request)
        {
            var filter = request.Filter ?? new StudentFilterDto();

            // Build base query: students that have progress entries for this halqa
            var baseQuery = _context.Students
                .Where(s => s.progressEntrys.Any(pe => pe.HalqaId == request.HalqaId));

            // Apply filters (will be translated to SQL)
            if (!string.IsNullOrWhiteSpace(filter.Name))
                baseQuery = baseQuery.Where(s => EF.Functions.Like(s.name, $"%{filter.Name}%"));

            if (!string.IsNullOrWhiteSpace(filter.Gender))
                baseQuery = baseQuery.Where(s => s.gender == filter.Gender);

            if (filter.AgeFrom.HasValue)
                baseQuery = baseQuery.Where(s => s.age >= filter.AgeFrom.Value);

            if (filter.AgeTo.HasValue)
                baseQuery = baseQuery.Where(s => s.age <= filter.AgeTo.Value);

            if (filter.Status.HasValue)
                baseQuery = baseQuery.Where(s => s.status == filter.Status.Value);

            if (filter.MinScore.HasValue)
                baseQuery = baseQuery.Where(s => s.score >= filter.MinScore.Value);

            if (filter.MaxScore.HasValue)
                baseQuery = baseQuery.Where(s => s.score <= filter.MaxScore.Value);

            int page = Math.Max(1, filter.PageNumber);
            int pageSize = Math.Max(1, filter.PageSize);
            int skip = (page - 1) * pageSize;

            var totalCount = await baseQuery.CountAsync();

            var students = await baseQuery
                .OrderBy(s => s.name)
                .Skip(skip)
                .Take(pageSize)
                .Select(s => new StudentDto
                {
                    Id = s.Id,
                    Name = s.name,
                    Email = s.Email,
                    Phone = s.PhoneNumber,
                    Gender = s.gender,
                    FontSize = s.font_size,
                    Role = s.role,
                    Theme = s.theme,
                    ProfileImageUrl = s.profile_imageUrl,
                    CreatedAt = s.created_at,
                    JoinedAt = s.JoinedAt,
                    Age = s.age,
                    EnrollmentDate = s.EnrollmentDate,
                    Status = s.status,
                    Score = s.score,
                    MosqueId = s.MosqueId,
                    SaturdayHalqeId = s.SaturdayHalqaId
                })
                .ToListAsync();

            return new HalqaStudentsResponse
            {
                Students = students,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<HalqaStudentsCountResponse> GetHalqaStudentsCountAsync(GetHalqaStudentsCountRequest request)
        {
            // Count distinct students who have progress entries for the halqa
            var count = await _context.ProgressEntries
                .Where(pe => pe.HalqaId == request.CircleId)
                .Select(pe => pe.studentId)
                .Distinct()
                .CountAsync();

            return new HalqaStudentsCountResponse { Count = count };
        }

        public async Task<HalqaStatisticsDto> GetHalqaStatisticsAsync(GetHalqaStatisticsRequest request)
        {
            var circleId = request.HalqaId;

            // 1) students count and active students count (two counts, executed in DB)
            var studentsCountTask = _context.ProgressEntries
                .Where(pe => pe.HalqaId == circleId)
                .Select(pe => pe.StudentId)
                .Distinct()
                .CountAsync();

            var activeCountTask = _context.Students
                .Where(s => s.progressEntrys.Any(pe => pe.HalqaId == circleId) && s.status != 0)
                .Select(s => s.Id)
                .Distinct()
                .CountAsync();

            // 2) average memorization progress (single query)
            var avgMemTask = _context.ProgressEntries
                .Where(pe => pe.HalqaId == circleId)
                .Select(pe => (double?)pe.MemorizedUntil)
                .AverageAsync();

            // 3) sessions in optional range and attendance counts (fetch sessions ids then count attendances)
            var sessionsQuery = _context.HalqaSessions.Where(hs => hs.HalqaId == circleId);
            if (request.FromDate.HasValue) sessionsQuery = sessionsQuery.Where(hs => hs.date >= request.FromDate.Value);
            if (request.ToDate.HasValue) sessionsQuery = sessionsQuery.Where(hs => hs.date <= request.ToDate.Value);

            var sessionsListTask = sessionsQuery
                .OrderBy(hs => hs.date)
                .Select(hs => hs.Id)
                .ToListAsync();

            await Task.WhenAll(studentsCountTask, activeCountTask, avgMemTask, sessionsListTask);

            int studentsCount = studentsCountTask.Result;
            int activeCount = activeCountTask.Result;
            double avgMem = avgMemTask.Result ?? 0.0;
            var sessionIds = sessionsListTask.Result;

            double attendanceRate = 0.0;
            if (sessionIds.Any() && studentsCount > 0)
            {
                var presentCount = await _context.Attendances
                    .Where(a => sessionIds.Contains(a.HalqeSessionId))
                    .CountAsync();

                attendanceRate = (presentCount / (double)(studentsCount * sessionIds.Count)) * 100.0;
                attendanceRate = Math.Clamp(attendanceRate, 0.0, 100.0);
            }

            var halqa = await _context.Halqas.FindAsync(circleId);

            return new HalqaStatisticsDto
            {
                HalqaId = circleId,
                HalqaName = halqa?.Name ?? string.Empty,
                StudentsCount = studentsCount,
                ActiveStudentsCount = activeCount,
                AverageMemorizationProgress = Math.Round(avgMem, 2),
                AttendanceRate = Math.Round(attendanceRate, 2),
                AverageEvaluationScore = 0.0 // يحتاج مصدر تقييم خارجي، نترك 0 الآن
            };
        }

        public async Task<HalqaAttendanceReportResponse> GetHalqaAttendanceReportAsync(GetHalqaAttendanceReportRequest request)
        {
            // 1) جلب الجلسات ضمن الفترة
            var sessions = await _context.HalqaSessions
                .Where(hs => hs.HalqaId == request.HalqaId && hs.date >= request.FromDate && hs.date <= request.ToDate)
                .OrderBy(hs => hs.date)
                .Select(hs => new { hs.Id, hs.date })
                .ToListAsync();

            var sessionIds = sessions.Select(s => s.Id).ToList();

            // 2) جلب الحضور مرة واحدة، مجمّع حسب HalqeSessionId
            var attendanceGroups = new Dictionary<Guid, int>();
            if (sessionIds.Any())
            {
                var groups = await _context.Attendances
                    .Where(a => sessionIds.Contains(a.HalqeSessionId))
                    .GroupBy(a => a.HalqeSessionId)
                    .Select(g => new { SessionId = g.Key, Count = g.Count() })
                    .ToListAsync();

                attendanceGroups = groups.ToDictionary(g => g.SessionId, g => g.Count);
            }

            var halqa = await _context.Halqas.FindAsync(request.HalqaId);

            var result = new HalqaAttendanceReportResponse
            {
                HalqaId = request.HalqaId,
                HalqaName = halqa?.Name ?? string.Empty,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                DailyRecords = new List<HalqaAttendanceDailyRecordDto>()
            };

            foreach (var s in sessions)
            {
                attendanceGroups.TryGetValue(s.Id, out int presentCount);

                result.DailyRecords.Add(new HalqaAttendanceDailyRecordDto
                {
                    Date = s.date.Date,
                    PresentCount = presentCount,
                    AbsentCount = 0,
                    LateCount = 0,
                    ExcusedCount = 0
                });
            }

            return result;
        }
    }
}
