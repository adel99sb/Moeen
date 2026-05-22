using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.StudentNotes;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.StudentNotes;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class StudentNotesService : IStudentNotesService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public StudentNotesService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> AddStudentNoteAsync(AddStudentNoteRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty || string.IsNullOrWhiteSpace(request.Text))
                return GeneralResponse.BadRequest("»Ì«‰«  «·„·«ÕŸ… €Ì— ’«·Õ….");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("€Ì— „’—Õ.");

            var student = await _context.Students.FindAsync(request.StudentId);
            if (student == null)
                return GeneralResponse.NotFound("«·ÿ«·» €Ì— „ÊÃÊœ.");

            var circleId = await ResolveCircleIdAsync(teacherId.Value);
            if (!circleId.HasValue)
                return GeneralResponse.NotFound("·«  ÊÃœ Õ·ﬁ… „— »ÿ… »«·„⁄·„.");

            if (student.SaturdayHalqeId == Guid.Empty)
                return GeneralResponse.BadRequest("·«  ÊÃœ Õ·ﬁ… ”»  „— »ÿ… »«·ÿ«·».");

            var date = (request.Date ?? DateTime.UtcNow).Date;

            var session = await _context.HalqaSessions
                .FirstOrDefaultAsync(s => s.HalqaId == circleId.Value && s.date == date);

            if (session == null)
            {
                session = new HalqaSession
                {
                    Id = Guid.NewGuid(),
                    HalqaId = circleId.Value,
                    date = date,
                    start_time = TimeSpan.Zero,
                    end_time = TimeSpan.Zero
                };

                await _context.HalqaSessions.AddAsync(session);
            }

            var lesson = await _context.Set<SaturdayLesson>()
                .FirstOrDefaultAsync(l => l.SaturdayHalqeId == student.SaturdayHalqeId && l.lesson_number == 0);

            if (lesson == null)
            {
                lesson = new SaturdayLesson
                {
                    Id = Guid.NewGuid(),
                    SaturdayHalqeId = student.SaturdayHalqeId,
                    lesson_number = 0,
                    start_time = TimeSpan.Zero,
                    end_time = TimeSpan.Zero
                };

                await _context.Set<SaturdayLesson>().AddAsync(lesson);
            }

            var attendance = await _context.Attendances
                .FirstOrDefaultAsync(a => a.StudentId == student.Id && a.HalqeSessionId == session.Id);

            if (attendance == null)
            {
                attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    StudentId = student.Id,
                    TeacherId = teacherId.Value,
                    HalqeSessionId = session.Id,
                    SaturdayLessonId = lesson.Id,
                    Status = AttendanceStatus.Present,
                    Note = request.Text.Trim()
                };

                await _context.Attendances.AddAsync(attendance);
            }
            else
            {
                attendance.Note = request.Text.Trim();
            }

            await _context.SaveChangesAsync();

            var teacher = await _context.Teachers.FindAsync(teacherId.Value);

            return GeneralResponse.Ok(" „  ≈÷«›… «·„·«ÕŸ….", new StudentNoteDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                TeacherId = attendance.TeacherId,
                TeacherName = teacher?.name ?? string.Empty,
                Text = attendance.Note ?? string.Empty,
                Date = session.date
            });
        }

        public async Task<GeneralResponse> GetStudentNotesAsync(GetStudentNotesRequest request)
        {
            if (request == null || request.StudentId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·ÿ«·» „ÿ·Ê».");

            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.Teacher)
                .Include(a => a.HalqeSession)
                .Where(a => a.StudentId == request.StudentId && !string.IsNullOrWhiteSpace(a.Note));

            if (request.FromDate.HasValue)
                query = query.Where(a => a.HalqeSession.date >= request.FromDate.Value.Date);

            if (request.ToDate.HasValue)
                query = query.Where(a => a.HalqeSession.date <= request.ToDate.Value.Date);

            var total = await query.CountAsync();
            var page = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var items = await query
                .OrderByDescending(a => a.HalqeSession.date)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new StudentNoteDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    TeacherId = a.TeacherId,
                    TeacherName = a.Teacher.name,
                    Text = a.Note,
                    Date = a.HalqeSession.date
                })
                .ToListAsync();

            return GeneralResponse.Ok(" „ Ã·» «·„·«ÕŸ« .", items, page, pageSize, total);
        }

        public async Task<GeneralResponse> UpdateStudentNoteAsync(UpdateStudentNoteRequest request)
        {
            if (request == null || request.NoteId == Guid.Empty || string.IsNullOrWhiteSpace(request.Text))
                return GeneralResponse.BadRequest("»Ì«‰«  «·„·«ÕŸ… €Ì— ’«·Õ….");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("€Ì— „’—Õ.");

            var attendance = await _context.Attendances
                .Include(a => a.Teacher)
                .Include(a => a.HalqeSession)
                .FirstOrDefaultAsync(a => a.Id == request.NoteId);

            if (attendance == null)
                return GeneralResponse.NotFound("«·„·«ÕŸ… €Ì— „ÊÃÊœ….");

            if (attendance.TeacherId != teacherId.Value)
                return GeneralResponse.Unauthorized("·«  „·ﬂ ’·«ÕÌ…  ⁄œÌ· Â–Â «·„·«ÕŸ….");

            attendance.Note = request.Text.Trim();
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „  ÕœÌÀ «·„·«ÕŸ….", new StudentNoteDto
            {
                Id = attendance.Id,
                StudentId = attendance.StudentId,
                TeacherId = attendance.TeacherId,
                TeacherName = attendance.Teacher?.name ?? string.Empty,
                Text = attendance.Note ?? string.Empty,
                Date = attendance.HalqeSession?.date ?? DateTime.UtcNow
            });
        }

        public async Task<GeneralResponse> DeleteStudentNoteAsync(Guid noteId)
        {
            if (noteId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·„·«ÕŸ… „ÿ·Ê».");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("€Ì— „’—Õ.");

            var attendance = await _context.Attendances.FindAsync(noteId);
            if (attendance == null)
                return GeneralResponse.NotFound("«·„·«ÕŸ… €Ì— „ÊÃÊœ….");

            if (attendance.TeacherId != teacherId.Value)
                return GeneralResponse.Unauthorized("·«  „·ﬂ ’·«ÕÌ… Õ–› Â–Â «·„·«ÕŸ….");

            attendance.Note = null;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „ Õ–› «·„·«ÕŸ….");
        }

        private async Task<Guid?> ResolveCircleIdAsync(Guid teacherId)
        {
            return await _context.Halqas
                .AsNoTracking()
                .Where(h => h.TeacherId == teacherId)
                .Select(h => (Guid?)h.Id)
                .FirstOrDefaultAsync();
        }
    }
}