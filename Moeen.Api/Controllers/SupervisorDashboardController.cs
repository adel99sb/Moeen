using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.SupervisorDashboard;
using System.Security.Claims;

namespace Moeen.Api.Controllers
{
    [Route("api/supervisor-dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin,Owner,Supervisor")]
    public class SupervisorDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<SupervisorDashboardController> _logger;

        public SupervisorDashboardController(AppDbContext context, ILogger<SupervisorDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<SupervisorDashboardResponse>> GetOverview()
        {
            _logger.LogInformation(
                "Supervisor dashboard overview called. IsAuthenticated={IsAuthenticated}, HasAuthorizationHeader={HasAuthorizationHeader}, UserIdClaim={UserIdClaim}, NameIdentifier={NameIdentifier}, Roles={Roles}",
                User.Identity?.IsAuthenticated,
                Request.Headers.ContainsKey("Authorization"),
                User.FindFirstValue("UserIdentifier"),
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                string.Join(",", User.Claims.Where(c => c.Type == ClaimTypes.Role || c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) || c.Type.EndsWith("/role", StringComparison.OrdinalIgnoreCase)).Select(c => c.Value)));

            var userId = GetCurrentUserId();
            if (userId == null)
            {
                _logger.LogWarning(
                    "Supervisor dashboard overview rejected because user id claim was missing or invalid. AvailableClaimTypes={ClaimTypes}",
                    string.Join(",", User.Claims.Select(c => c.Type).Distinct()));
                return Unauthorized("تعذر تحديد المستخدم الحالي من التوكن.");
            }

            var mosqueId = await ResolveMosqueIdAsync(userId.Value);
            if (mosqueId == null)
                return NotFound("لم يتم العثور على مسجد مرتبط بالمستخدم الحالي.");

            var fromDate = DateTime.UtcNow.Date.AddDays(-30);
            var mosqueUserIds = await GetMosqueUserIdsAsync(mosqueId.Value);

            var halqas = await _context.Halqas
                .AsNoTracking()
                .Where(h => h.Fouj.MosqueId == mosqueId.Value)
                .OrderBy(h => h.Name)
                .Take(8)
                .Select(h => new HalqaLookup(h.Id, h.Name))
                .ToListAsync();

            var response = new SupervisorDashboardResponse
            {
                TotalStudents = await _context.Students.AsNoTracking().CountAsync(s => s.MosqueId == mosqueId.Value),
                TotalTeachers = await _context.Teachers.AsNoTracking().CountAsync(t => t.MosqueId == mosqueId.Value),
                TotalHalqas = await _context.Halqas.AsNoTracking().CountAsync(h => h.Fouj.MosqueId == mosqueId.Value),
                OpenAlertsAndComplaints = await _context.Complaints.AsNoTracking().CountAsync(c =>
                    mosqueUserIds.Contains(c.UserId) &&
                    c.Status != ComplaintStatus.Resolved &&
                    c.Status != ComplaintStatus.Delete),
                HalqaPerformance = await BuildHalqaPerformanceAsync(halqas, fromDate),
                ExcellentStudents = await BuildExcellentStudentsAsync(mosqueId.Value),
                StrugglingStudents = await BuildStrugglingStudentsAsync(mosqueId.Value, fromDate)
            };

            return Ok(response);
        }

        private Guid? GetCurrentUserId()
        {
            var rawId = User.FindFirstValue("UserIdentifier")
                        ?? User.FindFirstValue(ClaimTypes.NameIdentifier)
                        ?? User.FindFirstValue("sub")
                        ?? User.FindFirstValue("nameid");

            return Guid.TryParse(rawId, out var userId) ? userId : null;
        }

        private async Task<Guid?> ResolveMosqueIdAsync(Guid userId)
        {
            var supervisorMosqueId = await _context.Supervisors
                .AsNoTracking()
                .Where(s => s.Id == userId)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync();

            if (supervisorMosqueId.HasValue)
                return supervisorMosqueId;

            return await _context.Mosques
                .AsNoTracking()
                .OrderBy(m => m.name)
                .Select(m => (Guid?)m.Id)
                .FirstOrDefaultAsync();
        }

        private async Task<List<Guid>> GetMosqueUserIdsAsync(Guid mosqueId)
        {
            var studentIds = _context.Students.AsNoTracking().Where(s => s.MosqueId == mosqueId).Select(s => s.Id);
            var teacherIds = _context.Teachers.AsNoTracking().Where(t => t.MosqueId == mosqueId).Select(t => t.Id);
            var supervisorIds = _context.Supervisors.AsNoTracking().Where(s => s.MosqueId == mosqueId).Select(s => s.Id);

            return await studentIds.Concat(teacherIds).Concat(supervisorIds).Distinct().ToListAsync();
        }

        private async Task<List<HalqaPerformanceDto>> BuildHalqaPerformanceAsync(IEnumerable<HalqaLookup> halqas, DateTime fromDate)
        {
            var result = new List<HalqaPerformanceDto>();

            foreach (var halqa in halqas)
            {
                var tests = await _context.Exams.AsNoTracking()
                    .CountAsync(e => e.date >= fromDate && e.Student.HalqaId == halqa.Id);

                var recitations = await _context.ProgressEntries.AsNoTracking()
                    .CountAsync(p => !p.IsDeleted && p.Date >= fromDate && p.HalqaId == halqa.Id);

                var attendance = await _context.Attendances.AsNoTracking()
                    .CountAsync(a =>
                        a.HalqeSession.HalqaId == halqa.Id &&
                        a.HalqeSession.date >= fromDate &&
                        (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late));

                result.Add(new HalqaPerformanceDto
                {
                    HalqaName = halqa.Name,
                    Tests = tests,
                    Recitations = recitations,
                    Attendance = attendance
                });
            }

            return result;
        }

        private async Task<List<DashboardStudentDto>> BuildExcellentStudentsAsync(Guid mosqueId)
        {
            return await _context.Students
                .AsNoTracking()
                .Where(s => s.MosqueId == mosqueId)
                .OrderByDescending(s => s.score)
                .ThenBy(s => s.name)
                .Take(5)
                .Select(s => new DashboardStudentDto
                {
                    StudentId = s.Id,
                    StudentName = s.name,
                    Score = s.score,
                    Badge = s.score >= 90 ? "ممتاز" : "متميز",
                    Reason = "من أعلى الطلاب نقاطاً خلال الفترة الحالية"
                })
                .ToListAsync();
        }

        private async Task<List<DashboardStudentDto>> BuildStrugglingStudentsAsync(Guid mosqueId, DateTime fromDate)
        {
            var absenceCounts = await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    a.HalqeSession.date >= fromDate &&
                    a.Status == AttendanceStatus.Absent &&
                    a.Student.MosqueId == mosqueId)
                .GroupBy(a => a.StudentId)
                .Select(g => new { StudentId = g.Key, Absences = g.Count() })
                .ToDictionaryAsync(x => x.StudentId, x => x.Absences);

            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.MosqueId == mosqueId)
                .OrderBy(s => s.score)
                .ThenBy(s => s.name)
                .Take(5)
                .Select(s => new { s.Id, s.name, s.score })
                .ToListAsync();

            return students.Select(s =>
            {
                absenceCounts.TryGetValue(s.Id, out var absences);
                return new DashboardStudentDto
                {
                    StudentId = s.Id,
                    StudentName = s.name,
                    Score = s.score,
                    Badge = absences >= 3 ? "تنبيه غياب" : "متابعة خطة",
                    Reason = absences >= 3
                        ? $"غياب متكرر عن الحصص ({absences} أيام)"
                        : "يحتاج متابعة بسبب انخفاض النقاط الحالية"
                };
            }).ToList();
        }

        private sealed record HalqaLookup(Guid Id, string Name);
    }
}
