using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses.OwnerDashboard;

namespace Moeen.Api.Controllers
{
    [Route("api/owner-dashboard")]
    [ApiController]
    [Authorize(Roles = "Admin,Owner")]
    public class OwnerDashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<OwnerDashboardController> _logger;

        public OwnerDashboardController(AppDbContext context, ILogger<OwnerDashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("overview")]
        public async Task<ActionResult<OwnerDashboardResponse>> GetOverview()
        {
            _logger.LogInformation("Owner dashboard overview requested. HasAuthorizationHeader={HasAuthorizationHeader}", Request.Headers.ContainsKey("Authorization"));

            var response = new OwnerDashboardResponse
            {
                TotalMosques = await _context.Mosques.AsNoTracking().CountAsync(),
                TotalSupervisors = await _context.Supervisors.AsNoTracking().CountAsync(),
                TotalTeachers = await _context.Teachers.AsNoTracking().CountAsync(t => t.status == 0),
                TotalStudents = await _context.Students.AsNoTracking().CountAsync(s => s.role == 2 && s.status == 0),
                TotalHalqas = await _context.Halqas.AsNoTracking().CountAsync(),
                TotalOpenComplaints = await _context.Complaints.AsNoTracking().CountAsync(c =>
                    (c.Type == FeedbackType.Complaint && c.Status == ComplaintStatus.TransferredToOwner) ||
                    (c.Type == FeedbackType.Suggestion && c.SuggestionStatus == SuggestionStatus.TransferredToOwner)),
                TotalPosts = await _context.Posts.AsNoTracking().CountAsync(),
                TotalBooks = await _context.PdfFiles.AsNoTracking().CountAsync()
            };

            response.MosqueSummaries = await BuildMosqueSummariesAsync();
            response.RecentActivities = await BuildRecentActivitiesAsync();
            response.Alerts = BuildAlerts(response);

            return Ok(response);
        }

        private async Task<List<OwnerMosqueSummaryDto>> BuildMosqueSummariesAsync()
        {
            var mosques = await _context.Mosques
                .AsNoTracking()
                .OrderBy(m => m.name)
                .Take(6)
                .Select(m => new { m.Id, m.name, m.address })
                .ToListAsync();

            var summaries = new List<OwnerMosqueSummaryDto>();
            foreach (var mosque in mosques)
            {
                var userIds = await _context.Students.AsNoTracking().Where(s => s.MosqueId == mosque.Id && s.role == 2 && s.status == 0).Select(s => s.Id)
                    .Concat(_context.Teachers.AsNoTracking().Where(t => t.MosqueId == mosque.Id && t.status == 0).Select(t => t.Id))
                    .Concat(_context.Supervisors.AsNoTracking().Where(s => s.MosqueId == mosque.Id).Select(s => s.Id))
                    .Distinct()
                    .ToListAsync();

                summaries.Add(new OwnerMosqueSummaryDto
                {
                    MosqueId = mosque.Id,
                    MosqueName = mosque.name,
                    Address = mosque.address,
                    Supervisors = await _context.Supervisors.AsNoTracking().CountAsync(s => s.MosqueId == mosque.Id),
                    Teachers = await _context.Teachers.AsNoTracking().CountAsync(t => t.MosqueId == mosque.Id && t.status == 0),
                    Students = await _context.Students.AsNoTracking().CountAsync(s => s.MosqueId == mosque.Id && s.role == 2 && s.status == 0),
                    Halqas = await _context.Halqas.AsNoTracking().CountAsync(h => h.Fouj.MosqueId == mosque.Id),
                    OpenComplaints = userIds.Count == 0
                        ? 0
                        : await _context.Complaints.AsNoTracking().CountAsync(c =>
                            userIds.Contains(c.UserId) &&
                            ((c.Type == FeedbackType.Complaint && c.Status == ComplaintStatus.TransferredToOwner) ||
                             (c.Type == FeedbackType.Suggestion && c.SuggestionStatus == SuggestionStatus.TransferredToOwner)))
                });
            }

            return summaries;
        }

        private async Task<List<OwnerActivityItemDto>> BuildRecentActivitiesAsync()
        {
            var posts = await _context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.created_at)
                .Take(3)
                .Select(p => new OwnerActivityItemDto
                {
                    Title = p.title,
                    Description = p.Mosque.name,
                    Icon = "bi bi-megaphone-fill",
                    CreatedAt = p.created_at
                })
                .ToListAsync();

            var forwardedFeedback = await _context.Complaints
                .AsNoTracking()
                .Where(c =>
                    (c.Type == FeedbackType.Complaint && c.Status == ComplaintStatus.TransferredToOwner) ||
                    (c.Type == FeedbackType.Suggestion && c.SuggestionStatus == SuggestionStatus.TransferredToOwner))
                .OrderByDescending(c => c.UpdatedAt ?? c.created_at)
                .Take(3)
                .Select(c => new OwnerActivityItemDto
                {
                    Title = c.Type == FeedbackType.Complaint ? "شكوى محولة" : "اقتراح محول",
                    Description = c.content,
                    Icon = "bi bi-chat-dots-fill",
                    CreatedAt = c.UpdatedAt ?? c.created_at
                })
                .ToListAsync();

            return posts.Concat(forwardedFeedback)
                .OrderByDescending(item => item.CreatedAt)
                .Take(6)
                .ToList();
        }

        private static List<OwnerAlertDto> BuildAlerts(OwnerDashboardResponse response)
        {
            var alerts = new List<OwnerAlertDto>();

            if (response.TotalMosques == 0)
            {
                alerts.Add(new OwnerAlertDto
                {
                    Title = "لا توجد مساجد مسجلة",
                    Details = "ابدأ بإضافة مسجد حتى تظهر بقية إحصائيات المنصة.",
                    Severity = "warning"
                });
            }

            if (response.TotalOpenComplaints > 0)
            {
                alerts.Add(new OwnerAlertDto
                {
                    Title = "شكاوى واقتراحات محولة تحتاج متابعة",
                    Details = $"يوجد {response.TotalOpenComplaints} شكوى أو اقتراح محول للمالك.",
                    Severity = "danger"
                });
            }

            if (response.TotalTeachers == 0 || response.TotalStudents == 0)
            {
                alerts.Add(new OwnerAlertDto
                {
                    Title = "بيانات تعليمية غير مكتملة",
                    Details = "تأكد من إضافة المعلمين والطلاب حتى تعمل الإحصائيات بشكل كامل.",
                    Severity = "info"
                });
            }

            return alerts;
        }
    }
}
