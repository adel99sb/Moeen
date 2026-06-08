using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.DailyAssignments;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.DailyAssignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class DailyAssignmentService : IDailyAssignmentService
    {
        private const int PagesPerJuz = 20;
        private const int ListeningTarget = -1;

        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public DailyAssignmentService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> GetDailyAssignmentsAsync(GetDailyAssignmentsRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·ÿ«·» „ÿ·Ê».");

            var date = (request.Date ?? DateTime.UtcNow).Date;

            var query = _context.ProgressEntries
                .AsNoTracking()
                .Where(p => p.StudentId == request.StudentId && p.Date.Date == date);

            if (request.HalqaId.HasValue && request.HalqaId.Value != Guid.Empty)
                query = query.Where(p => p.HalqaId == request.HalqaId.Value);

            var entries = await query.ToListAsync();
            var result = entries.Select(MapAssignment).ToList();

            return GeneralResponse.Ok(" „ Ã·» «·Ê«Ã»«  «·ÌÊ„Ì….", result);
        }

        public async Task<GeneralResponse> UpdateAssignmentStatusAsync(UpdateAssignmentStatusRequest request)
        {
            if (request == null || request.AssignmentId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·Ê«Ã» „ÿ·Ê».");

            var entry = await _context.ProgressEntries.FindAsync(request.AssignmentId);
            if (entry == null)
                return GeneralResponse.NotFound("«·Ê«Ã» €Ì— „ÊÃÊœ.");

            var currentType = ResolveType(entry);
            if (currentType != request.AssignmentType)
                return GeneralResponse.BadRequest("‰Ê⁄ «·Ê«Ã» €Ì— „ÿ«»ﬁ ··”Ã·.");

            switch (request.Status)
            {
                case ReviewType.Submitted:
                    entry.LevelScore = 0;
                    entry.IsDeleted = false;
                    entry.DeletedAt = null;
                    break;

                case ReviewType.Evaluated:
                    if (!request.Grade.HasValue)
                        return GeneralResponse.BadRequest("«· ﬁœÌ— „ÿ·Ê» ·· ﬁÌÌ„.");
                    entry.LevelScore = MapGradeToPoints(request.Grade.Value);
                    entry.IsDeleted = false;
                    entry.DeletedAt = null;
                    break;

                case ReviewType.Cancelled:
                    entry.IsDeleted = true;
                    entry.DeletedAt = DateTime.UtcNow;
                    break;

                default:
                    return GeneralResponse.BadRequest("Õ«·… «·Ê«Ã» €Ì— ’«·Õ….");
            }

            await _context.SaveChangesAsync();
            return GeneralResponse.Ok(" „  ÕœÌÀ Õ«·… «·Ê«Ã».", MapAssignment(entry));
        }

        public async Task<GeneralResponse> AddDailyAssignmentAsync(AddDailyAssignmentRequest request)
        {
            if (request == null || request.StudentIds == null || request.StudentIds.Count == 0)
                return GeneralResponse.BadRequest("ﬁ«∆„… «·ÿ·«» „ÿ·Ê»….");

            if (request.AssignmentType == ProgressRecordType.Exam)
                return GeneralResponse.BadRequest("‰Ê⁄ «·Ê«Ã» €Ì— ’«·Õ.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("€Ì— „’—Õ.");

            if (request.AssignmentType != ProgressRecordType.Listening)
            {
                if (!request.FromPage.HasValue || !request.ToPage.HasValue)
                    return GeneralResponse.BadRequest("‰ÿ«ﬁ «·’›Õ«  „ÿ·Ê».");

                if (request.FromPage.Value <= 0 || request.ToPage.Value < request.FromPage.Value)
                    return GeneralResponse.BadRequest("‰ÿ«ﬁ «·’›Õ«  €Ì— ’«·Õ.");
            }

            var date = (request.Date ?? DateTime.UtcNow).Date;
            var studentIds = request.StudentIds.Distinct().ToList();

            var students = await _context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync();

            if (students.Count != studentIds.Count)
                return GeneralResponse.NotFound("»⁄÷ «·ÿ·«» €Ì— „ÊÃÊœÌ‰.");

            var entries = new List<ProgressEntry>();

            foreach (var student in students)
            {
                var halqaId = await ResolveHalqaIdAsync(teacherId.Value, student.SaturdayHalqeId);
                if (halqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("·«  ÊÃœ Õ·ﬁ… „— »ÿ… »«·ÿ«·».");

                var fromPage = request.AssignmentType == ProgressRecordType.Listening ? 0 : request.FromPage!.Value;
                var toPage = request.AssignmentType == ProgressRecordType.Listening ? 0 : request.ToPage!.Value;

                var entry = new ProgressEntry
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.Id,
                    TeacherId = teacherId.Value,
                    HalqaId = halqaId,
                    Date = date,
                    JuzNumber = request.AssignmentType == ProgressRecordType.Listening
                        ? 0
                        : (request.JuzNumber ?? CalculateJuzNumber(fromPage)),
                    PageNumber = fromPage,
                    MemorizedUntil = toPage,
                    NextTarget = request.AssignmentType switch
                    {
                        ProgressRecordType.Memorization => Math.Max(toPage + 1, 0),
                        ProgressRecordType.Review => 0,
                        _ => ListeningTarget
                    },
                    LevelScore = 0,
                    IsDeleted = false,
                    DeletedAt = null
                };

                entries.Add(entry);
            }

            await _context.ProgressEntries.AddRangeAsync(entries);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „ ≈÷«›… «·Ê«Ã»«  «·ÌÊ„Ì….", entries.Select(MapAssignment).ToList());
        }

        private static DailyAssignmentDto MapAssignment(ProgressEntry entry)
        {
            var status = ResolveStatus(entry);
            var points = entry.LevelScore;

            return new DailyAssignmentDto
            {
                AssignmentId = entry.Id,
                StudentId = entry.StudentId,
                Date = entry.Date,
                AssignmentType = ResolveType(entry),
                Status = status,
                JuzNumber = entry.JuzNumber,
                FromPage = entry.PageNumber,
                ToPage = entry.MemorizedUntil,
                Points = points,
                Grade = points > 0 ? ResolveGrade(points) : null
            };
        }

        private static ProgressRecordType ResolveType(ProgressEntry entry)
            => entry.NextTarget switch
            {
                0 => ProgressRecordType.Review,
                < 0 => ProgressRecordType.Listening,
                _ => ProgressRecordType.Memorization
            };

        private static ReviewType ResolveStatus(ProgressEntry entry)
            => entry.IsDeleted
                ? ReviewType.Cancelled
                : entry.LevelScore > 0
                    ? ReviewType.Evaluated
                    : ReviewType.Submitted;

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
    }
}