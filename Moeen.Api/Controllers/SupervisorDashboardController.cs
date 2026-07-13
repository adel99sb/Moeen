using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Application.Services;
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
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
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
            var studentPerformances = await BuildStudentAssessmentPerformanceAsync(mosqueId.Value, fromDate);

            var halqas = await _context.Halqas
                .AsNoTracking()
                .Where(h => h.Fouj.MosqueId == mosqueId.Value)
                .OrderBy(h => h.Name)
                .Take(8)
                .Select(h => new HalqaLookup(h.Id, h.Name))
                .ToListAsync();

            var response = new SupervisorDashboardResponse
            {
                TotalStudents = await _context.Students.AsNoTracking().CountAsync(s => s.MosqueId == mosqueId.Value && s.role == 2 && s.status == 0),
                TotalTeachers = await _context.Teachers.AsNoTracking().CountAsync(t => t.MosqueId == mosqueId.Value && t.status == 0),
                TotalHalqas = await _context.Halqas.AsNoTracking().CountAsync(h => h.Fouj.MosqueId == mosqueId.Value),
                OpenAlertsAndComplaints = await _context.Complaints.AsNoTracking().CountAsync(c =>
                    mosqueUserIds.Contains(c.UserId) &&
                    c.Type == FeedbackType.Complaint &&
                    c.Status != ComplaintStatus.Resolved &&
                    c.Status != ComplaintStatus.Delete &&
                    c.Status != ComplaintStatus.TransferredToOwner),
                HalqaPerformance = await BuildHalqaPerformanceAsync(halqas, fromDate),
                ExcellentStudents = SupervisorDashboardStudentClassifier.BuildExcellentStudents(studentPerformances),
                StrugglingStudents = SupervisorDashboardStudentClassifier.BuildStrugglingStudents(studentPerformances)
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

            return null;
        }

        private async Task<List<Guid>> GetMosqueUserIdsAsync(Guid mosqueId)
        {
            var studentIds = _context.Students.AsNoTracking().Where(s => s.MosqueId == mosqueId && s.role == 2 && s.status == 0).Select(s => s.Id);
            var teacherIds = _context.Teachers.AsNoTracking().Where(t => t.MosqueId == mosqueId && t.status == 0).Select(t => t.Id);
            var supervisorIds = _context.Supervisors.AsNoTracking().Where(s => s.MosqueId == mosqueId).Select(s => s.Id);

            return await studentIds.Concat(teacherIds).Concat(supervisorIds).Distinct().ToListAsync();
        }

        private async Task<List<HalqaPerformanceDto>> BuildHalqaPerformanceAsync(IEnumerable<HalqaLookup> halqas, DateTime fromDate)
        {
            var result = new List<HalqaPerformanceDto>();

            foreach (var halqa in halqas)
            {
                var tests = await _context.Exams.AsNoTracking()
                    .CountAsync(e => e.date >= fromDate && e.Student.HalqaId == halqa.Id && e.Student.status == 0);

                var recitations = await _context.ProgressEntries.AsNoTracking()
                    .CountAsync(p => !p.IsDeleted && p.Date >= fromDate && p.HalqaId == halqa.Id && p.Student.status == 0);

                var attendance = await _context.Attendances.AsNoTracking()
                    .CountAsync(a =>
                        a.HalqeSession.HalqaId == halqa.Id &&
                        a.Student.status == 0 &&
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

        private async Task<List<SupervisorDashboardStudentPerformance>> BuildStudentAssessmentPerformanceAsync(Guid mosqueId, DateTime fromDate)
        {
            var examScores = await _context.Exams
                .AsNoTracking()
                .Where(e => e.date >= fromDate && e.Student.MosqueId == mosqueId && e.Student.status == 0)
                .Select(e => new StudentAssessmentScore(e.StudentId, e.score))
                .ToListAsync();

            var recitationScores = await _context.ProgressEntries
                .AsNoTracking()
                .Where(p => !p.IsDeleted && p.Date >= fromDate && p.Student.MosqueId == mosqueId && p.Student.status == 0)
                .Select(p => new StudentAssessmentScore(p.StudentId, p.LevelScore))
                .ToListAsync();

            var performanceByStudent = examScores
                .Concat(recitationScores)
                .GroupBy(score => score.StudentId)
                .ToDictionary(
                    group => group.Key,
                    group => new
                    {
                        AverageScore = group.Average(score => score.Score),
                        AssessmentCount = group.Count()
                    });

            if (performanceByStudent.Count == 0)
                return new List<SupervisorDashboardStudentPerformance>();

            var studentIds = performanceByStudent.Keys.ToList();

            var students = await _context.Students
                .AsNoTracking()
                .Where(s => s.MosqueId == mosqueId && s.status == 0 && studentIds.Contains(s.Id))
                .Select(s => new { s.Id, s.name, s.score })
                .ToListAsync();

            return students.Select(s =>
            {
                var performance = performanceByStudent[s.Id];
                return new SupervisorDashboardStudentPerformance(
                    s.Id,
                    s.name ?? string.Empty,
                    s.score,
                    performance.AverageScore,
                    performance.AssessmentCount);
            }).ToList();
        }

        private sealed record HalqaLookup(Guid Id, string Name);
        private sealed record StudentAssessmentScore(Guid StudentId, int Score);
    }
}
