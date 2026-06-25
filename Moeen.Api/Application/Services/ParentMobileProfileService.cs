using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;

namespace Moeen.Api.Application.Services
{
    public class ParentMobileProfileService : IParentMobileProfileService
    {
        private readonly AppDbContext _context;

        public ParentMobileProfileService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> GetProfileAsync(Guid parentId)
        {
            if (parentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف ولي الأمر غير صالح.");

            var parent = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == parentId);

            if (parent == null)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            var children = await _context.Students
                .AsNoTracking()
                .Include(s => s.Mosque)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Fouj)
                .Include(s => s.Halqa)
                    .ThenInclude(h => h.Teacher)
                .Where(s => s.ParentId == parentId && s.status == 0)
                .OrderBy(s => s.name)
                .ToListAsync();

            var response = new ParentProfileResponse
            {
                ParentId = parent.Id,
                ParentName = parent.name ?? string.Empty,
                ParentInitials = BuildInitials(parent.name),
                Email = parent.Email ?? string.Empty,
                PhoneNumber = parent.PhoneNumber ?? string.Empty,
                Gender = NormalizeGender(parent.gender),
                JoinedAt = parent.JoinedAt == default ? parent.created_at == default ? null : parent.created_at : parent.JoinedAt,
                ChildrenCount = children.Count,
                TotalChildrenPoints = children.Sum(c => c.score),
                Children = children.Select(MapChild).ToList()
            };

            return GeneralResponse.Ok("تم جلب ملف ولي الأمر.", response);
        }

        public async Task<GeneralResponse> SubmitNoteAsync(Guid parentId, SubmitParentProfileNoteRequest request)
        {
            if (parentId == Guid.Empty)
                return GeneralResponse.BadRequest("معرف ولي الأمر غير صالح.");

            var content = request?.Content?.Trim() ?? string.Empty;
            if (content.Length < 5)
                return GeneralResponse.BadRequest("الملاحظة يجب أن تكون 5 أحرف على الأقل.");

            if (content.Length > 1000)
                return GeneralResponse.BadRequest("الملاحظة يجب ألا تتجاوز 1000 حرف.");

            var exists = await _context.Students.AsNoTracking().AnyAsync(s => s.Id == parentId);
            if (!exists)
                return GeneralResponse.NotFound("ولي الأمر غير موجود.");

            var complaint = new Complaint
            {
                Id = Guid.NewGuid(),
                UserId = parentId,
                Title = "ملاحظة من تطبيق ولي الأمر",
                content = content,
                created_at = DateTime.UtcNow,
                Type = FeedbackType.Suggestion,
                SuggestionStatus = SuggestionStatus.InProgress,
                Status = null,
                Response = null,
                UpdatedAt = null
            };

            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إرسال الملاحظة بنجاح.", new { complaint.Id });
        }

        private static ParentProfileChildDto MapChild(Student student)
        {
            return new ParentProfileChildDto
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                StudentInitials = BuildInitials(student.name),
                Age = student.age,
                Gender = NormalizeGender(student.gender),
                MosqueName = student.Mosque?.name ?? string.Empty,
                FoujName = student.Halqa?.Fouj?.name ?? string.Empty,
                HalqaName = student.Halqa?.Name ?? string.Empty,
                TeacherName = student.Halqa?.Teacher?.name ?? string.Empty,
                TotalPoints = student.score,
                Status = student.status,
                EnrollmentDate = student.EnrollmentDate == default ? null : student.EnrollmentDate
            };
        }

        private static string BuildInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "و";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
                return "و";

            return string.Concat(parts.Take(2).Select(part => part[0])).ToUpperInvariant();
        }

        private static string NormalizeGender(string? gender)
            => gender?.ToLowerInvariant() switch
            {
                "male" => "ذكر",
                "female" => "أنثى",
                _ => string.IsNullOrWhiteSpace(gender) ? "غير محدد" : gender
            };
    }
}
