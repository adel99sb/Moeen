using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Mobile;
using System.Text.Json;

namespace Moeen.Api.Application.Services
{
    public class ParentMobileProgressService : IParentMobileProgressService
    {
        private readonly AppDbContext _context;
        private readonly IStudentMobileProgressService _studentProgressService;

        public ParentMobileProgressService(AppDbContext context, IStudentMobileProgressService studentProgressService)
        {
            _context = context;
            _studentProgressService = studentProgressService;
        }

        public async Task<GeneralResponse> GetProgressAsync(Guid parentId, Guid? childId, DateTime? from, DateTime? to, ProgressRecordType? type)
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
                .Where(s => s.ParentId == parentId)
                .OrderBy(s => s.name)
                .ToListAsync();

            var response = new ParentProgressResponse
            {
                ParentId = parent.Id,
                ParentName = parent.name ?? string.Empty,
                Children = children.Select(MapChild).ToList()
            };

            if (children.Count == 0)
                return GeneralResponse.Ok("لا يوجد طلاب مرتبطون بحساب ولي الأمر.", response);

            var selectedChild = childId.HasValue
                ? children.FirstOrDefault(s => s.Id == childId.Value)
                : children.First();

            if (selectedChild == null)
                return GeneralResponse.Unauthorized("هذا الطالب غير مرتبط بحساب ولي الأمر الحالي.");

            var progressResult = await _studentProgressService.GetProgressAsync(selectedChild.Id, from, to, type);
            if (!progressResult.Success)
                return progressResult;

            response.SelectedStudentId = selectedChild.Id;
            response.SelectedStudentName = selectedChild.name ?? string.Empty;
            response.Progress = ExtractProgress(progressResult.Data) ?? new StudentProgressResponse
            {
                StudentId = selectedChild.Id,
                StudentName = selectedChild.name ?? string.Empty,
                StudentInitials = BuildInitials(selectedChild.name)
            };

            return GeneralResponse.Ok("تم جلب سجل تقدم الطالب لولي الأمر.", response);
        }

        private static StudentProgressResponse? ExtractProgress(object? data)
        {
            if (data is StudentProgressResponse progress)
                return progress;

            if (data == null)
                return null;

            var json = JsonSerializer.Serialize(data);
            return JsonSerializer.Deserialize<StudentProgressResponse>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        private static ParentChildDto MapChild(Student student)
        {
            return new ParentChildDto
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                StudentInitials = BuildInitials(student.name),
                MosqueName = student.Mosque?.name,
                HalqaName = student.Halqa?.Name,
                TotalPoints = student.score,
                Status = student.status
            };
        }

        private static string BuildInitials(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "ط";

            var parts = name.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
                return "ط";

            return string.Concat(parts.Take(2).Select(part => part[0])).ToUpperInvariant();
        }
    }
}
