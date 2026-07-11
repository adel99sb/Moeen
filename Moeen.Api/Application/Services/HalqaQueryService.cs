using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.HalqaQuery;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.Halqa;
using Moeen.Shared.Responses.HalqaQuery;

namespace Moeen.Api.Application.Services
{
    public class HalqaQueryService : IHalqaQueryService
    {
        private const int StudentRole = 2;

        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public HalqaQueryService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<HalqaDto>> GetAllHalqasAsync(Guid? mosqueId = null)
        {
            var query = _context.Halqas
                .AsNoTracking()
                .Include(h => h.Fouj)
                .Include(h => h.Teacher)
                .AsQueryable();

            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            var effectiveMosqueId = managedMosqueId ?? mosqueId;
            if (effectiveMosqueId.HasValue)
                query = query.Where(h => h.Fouj.MosqueId == effectiveMosqueId.Value);

            return await query
                .OrderBy(h => h.Fouj.name)
                .ThenBy(h => h.Name)
                .Select(h => new HalqaDto
                {
                    Id = h.Id,
                    Name = h.Name,
                    FoujId = h.FoujId,
                    FoujName = h.Fouj != null ? h.Fouj.name : string.Empty,
                    TeacherId = h.TeacherId ?? Guid.Empty,
                    TeacherName = h.Teacher != null ? h.Teacher.name : string.Empty,
                    Type = h.Type,
                    StudentsCount = h.Students.Count(s => s.role == StudentRole && s.status == 0)
                })
                .ToListAsync();
        }

        public async Task<List<HalqaAssignmentStudentOptionDto>> GetAssignmentStudentsAsync(Guid? halqaId = null)
        {
            var managedMosqueId = await ResolveManagedMosqueIdAsync();

            if (halqaId.HasValue && halqaId.Value != Guid.Empty)
            {
                var halqaExists = await _context.Halqas
                    .Include(h => h.Fouj)
                    .AnyAsync(h => h.Id == halqaId.Value && (!managedMosqueId.HasValue || h.Fouj.MosqueId == managedMosqueId.Value));

                if (!halqaExists)
                    throw new ArgumentException("الحلقة غير موجودة أو لا يمكنك إدارتها.");
            }

            var query = _context.Students
                .AsNoTracking()
                .Include(s => s.Halqa)
                .Where(s => s.role == StudentRole && s.status == 0);

            if (managedMosqueId.HasValue)
                query = query.Where(s => s.MosqueId == managedMosqueId.Value);

            if (halqaId.HasValue && halqaId.Value != Guid.Empty)
            {
                query = query.Where(s =>
                    !s.HalqaId.HasValue ||
                    s.HalqaId == Guid.Empty ||
                    s.HalqaId == halqaId.Value);
            }
            else
            {
                query = query.Where(s => !s.HalqaId.HasValue || s.HalqaId == Guid.Empty);
            }

            return await query
                .OrderBy(s => s.name)
                .Select(s => new HalqaAssignmentStudentOptionDto
                {
                    Id = s.Id,
                    Name = s.name ?? string.Empty,
                    Email = s.Email ?? string.Empty,
                    IsSelected = halqaId.HasValue && s.HalqaId == halqaId.Value,
                    HalqaId = s.HalqaId,
                    HalqaName = s.Halqa != null ? s.Halqa.Name : string.Empty
                })
                .ToListAsync();
        }

        public async Task<HalqaDto> GetHalqaByIdAsync(GetHalqaByIdRequest request)
        {
            var halqa = await _context.Halqas
                .Include(h => h.Fouj)
                .Include(h => h.Teacher)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.HalqaId));

            var studentsCount = await _context.Students
                .Where(s => s.role == StudentRole && s.status == 0 && s.HalqaId == request.HalqaId)
                .CountAsync();

            return new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = halqa.Fouj?.name,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = halqa.Teacher?.name,
                Type = halqa.Type,
                StudentsCount = studentsCount
            };
        }

        public async Task<HalqaStudentsResponse> GetHalqaStudentsAsync(GetHalqaStudentsRequest request)
        {
            var filter = request.Filter ?? new StudentFilterDto();

            var baseQuery = _context.Students
                .Where(s => s.role == StudentRole && s.status == 0 && s.HalqaId == request.HalqaId);

            if (!string.IsNullOrWhiteSpace(filter.Name))
                baseQuery = baseQuery.Where(s => EF.Functions.Like(s.name, $"%{filter.Name}%"));

            if (!string.IsNullOrWhiteSpace(filter.Gender))
                baseQuery = baseQuery.Where(s => s.gender == filter.Gender);

            if (filter.AgeFrom.HasValue)
                baseQuery = baseQuery.Where(s => s.age >= filter.AgeFrom.Value);

            if (filter.AgeTo.HasValue)
                baseQuery = baseQuery.Where(s => s.age <= filter.AgeTo.Value);

            if (filter.Status.HasValue && filter.Status.Value == 0)
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
            var count = await _context.Students
                .Where(s => s.role == StudentRole && s.status == 0 && s.HalqaId == request.HalqaId)
                .CountAsync();

            return new HalqaStudentsCountResponse { Count = count };
        }

        public async Task<HalqaStatisticsDto> GetHalqaStatisticsAsync(GetHalqaStatisticsRequest request)
        {
            var circleId = request.HalqaId;

            var studentsCountTask = _context.Students
                .Where(s => s.role == StudentRole && s.status == 0 && s.HalqaId == circleId)
                .CountAsync();

            var activeCountTask = _context.Students
                .Where(s => s.role == StudentRole && s.status == 0 && s.HalqaId == circleId)
                .CountAsync();

            var avgMemTask = _context.ProgressEntries
                .Where(pe => pe.HalqaId == circleId && pe.Student.status == 0)
                .Select(pe => (double?)pe.MemorizedUntil)
                .AverageAsync();

            var sessionsQuery = _context.HalqaSessions.Where(hs => hs.HalqaId == circleId);
            if (request.FromDate.HasValue)
                sessionsQuery = sessionsQuery.Where(hs => hs.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                sessionsQuery = sessionsQuery.Where(hs => hs.date <= request.ToDate.Value);

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
                    .Where(a => sessionIds.Contains(a.HalqeSessionId) && a.Student.status == 0)
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
                AverageEvaluationScore = 0.0
            };
        }

        public async Task<HalqaAttendanceReportResponse> GetHalqaAttendanceReportAsync(GetHalqaAttendanceReportRequest request)
        {
            var sessions = await _context.HalqaSessions
                .Where(hs => hs.HalqaId == request.HalqaId && hs.date >= request.FromDate && hs.date <= request.ToDate)
                .OrderBy(hs => hs.date)
                .Select(hs => new { hs.Id, hs.date })
                .ToListAsync();

            var sessionIds = sessions.Select(s => s.Id).ToList();

            var attendanceGroups = new Dictionary<Guid, int>();
            if (sessionIds.Any())
            {
                var groups = await _context.Attendances
                    .Where(a => sessionIds.Contains(a.HalqeSessionId) && a.Student.status == 0)
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

            foreach (var session in sessions)
            {
                attendanceGroups.TryGetValue(session.Id, out int presentCount);

                result.DailyRecords.Add(new HalqaAttendanceDailyRecordDto
                {
                    Date = session.date.Date,
                    PresentCount = presentCount,
                    AbsentCount = 0,
                    LateCount = 0,
                    ExcusedCount = 0
                });
            }

            return result;
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
    }
}
