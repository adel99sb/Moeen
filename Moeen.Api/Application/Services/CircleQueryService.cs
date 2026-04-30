using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.CircleQuery;
using Moeen.Shared.Responses.Circle;
using Moeen.Shared.Responses.CircleQuery;
using Moeen.Shared.Responses.Enrollment;

namespace Moeen.Api.Application.Services
{
    public class CircleQueryService : ICircleQueryService
    {
        private readonly AppDbContext _context;

        public CircleQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CircleDto> GetCircleByIdAsync(GetCircleByIdRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .Include(h => h.Teacher)
                .FirstOrDefaultAsync(h => h.Id == request.CircleId);

            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.CircleId));

            // Students count computed in DB (using ProgressEntry relation)
            var studentsCount = await _context.ProgressEntries
                .Where(pe => pe.HalqaId == request.CircleId)
                .Select(pe => pe.studentId)
                .Distinct()
                .CountAsync();

            return new CircleDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = halqa.Fouj?.name,
                TeacherId = halqa.TeacherId,
                TeacherName = halqa.Teacher?.name,
                Type = halqa.Type,

                StudentsCount = studentsCount
            };
        }

        public async Task<CircleStudentsResponse> GetCircleStudentsAsync(GetCircleStudentsRequest request)
        {
            var filter = request.Filter ?? new StudentFilterDto();

            // Build base query: students that have progress entries for this halqa
            var baseQuery = _context.Students
                .Where(s => s.progressEntrys.Any(pe => pe.HalqaId == request.CircleId));

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
                    EnrollmentDate = s.enrollmrnt_date,
                    Status = s.status,
                    Score = s.score,
                    MosqueId = s.MosqueId,
                    SaturdayHalqeId = s.SaturdayHalqaId
                })
                .ToListAsync();

            return new CircleStudentsResponse
            {
                Students = students,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<CircleStudentsCountResponse> GetCircleStudentsCountAsync(GetCircleStudentsCountRequest request)
        {
            // Count distinct students who have progress entries for the halqa
            var count = await _context.ProgressEntries
                .Where(pe => pe.HalqaId == request.CircleId)
                .Select(pe => pe.studentId)
                .Distinct()
                .CountAsync();

            return new CircleStudentsCountResponse { Count = count };
        }

        public async Task<CircleStatisticsDto> GetCircleStatisticsAsync(GetCircleStatisticsRequest request)
        {
            var circleId = request.CircleId;

            // 1) students count and active students count (two counts, executed in DB)
            var studentsCountTask = _context.ProgressEntries
                .Where(pe => pe.HalqaId == circleId)
                .Select(pe => pe.studentId)
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
                .Select(pe => (double?)pe.memorized_until)
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

            return new CircleStatisticsDto
            {
                CircleId = circleId,
                CircleName = halqa?.Name ?? string.Empty,
                StudentsCount = studentsCount,
                ActiveStudentsCount = activeCount,
                AverageMemorizationProgress = Math.Round(avgMem, 2),
                AttendanceRate = Math.Round(attendanceRate, 2),
                AverageEvaluationScore = 0.0 // يحتاج مصدر تقييم خارجي، نترك 0 الآن
            };
        }

        public async Task<CircleAttendanceReportResponse> GetCircleAttendanceReportAsync(GetCircleAttendanceReportRequest request)
        {
            // 1) جلب الجلسات ضمن الفترة
            var sessions = await _context.HalqaSessions
                .Where(hs => hs.HalqaId == request.CircleId && hs.date >= request.FromDate && hs.date <= request.ToDate)
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

            var halqa = await _context.Halqas.FindAsync(request.CircleId);

            var result = new CircleAttendanceReportResponse
            {
                CircleId = request.CircleId,
                CircleName = halqa?.Name ?? string.Empty,
                FromDate = request.FromDate,
                ToDate = request.ToDate,
                DailyRecords = new List<CircleAttendanceDailyRecordDto>()
            };

            foreach (var s in sessions)
            {
                attendanceGroups.TryGetValue(s.Id, out int presentCount);

                result.DailyRecords.Add(new CircleAttendanceDailyRecordDto
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
