using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Constants;
using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Attendance;
using System.Text;

namespace Moeen.Api.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public AttendanceService(AppDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<GeneralResponse> RecordDailyAttendanceAsync(RecordDailyAttendanceRequest request)
        {
            if (request?.Entries == null || request.Entries.Count == 0)
                return GeneralResponse.BadRequest("لا توجد بيانات حضور.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue)
                return GeneralResponse.Unauthorized("غير مصرح.");

            var circleId = await ResolveCircleIdAsync();
            if (!circleId.HasValue)
                return GeneralResponse.NotFound("لا توجد حلقة مرتبطة بالمعلم.");

            var entries = request.Entries;
            var studentIds = entries.Select(e => e.StudentId).Distinct().ToList();
            var dates = entries.Select(e => e.Date.Date).Distinct().ToList();

            var students = await _context.Students
                .Where(s => studentIds.Contains(s.Id))
                .ToListAsync();

            var studentLookup = students.ToDictionary(s => s.Id);

            var sessions = await _context.HalqaSessions
                .Where(s => s.HalqaId == circleId.Value && dates.Contains(s.date))
                .ToListAsync();

            var sessionLookup = sessions.ToDictionary(s => s.date.Date);
            var newSessions = new List<HalqaSession>();

            foreach (var date in dates)
            {
                if (sessionLookup.ContainsKey(date)) continue;

                var session = new HalqaSession
                {
                    Id = Guid.NewGuid(),
                    HalqaId = circleId.Value,
                    date = date,
                    start_time = TimeSpan.Zero,
                    end_time = TimeSpan.Zero
                };

                sessionLookup[date] = session;
                newSessions.Add(session);
            }

            var saturdayHalqaIds = students
                .Select(s => s.SaturdayHalqeId)
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToList();

            var lessons = await _context.Set<SaturdayLesson>()
                .Where(l => saturdayHalqaIds.Contains(l.SaturdayHalqeId) && l.lesson_number == 0)
                .ToListAsync();

            var lessonLookup = lessons.ToDictionary(l => l.SaturdayHalqeId);
            var newLessons = new List<SaturdayLesson>();

            foreach (var halqaId in saturdayHalqaIds)
            {
                if (lessonLookup.ContainsKey(halqaId)) continue;

                var lesson = new SaturdayLesson
                {
                    Id = Guid.NewGuid(),
                    SaturdayHalqeId = halqaId,
                    lesson_number = 0,
                    start_time = TimeSpan.Zero,
                    end_time = TimeSpan.Zero
                };

                lessonLookup[halqaId] = lesson;
                newLessons.Add(lesson);
            }

            var sessionIds = sessionLookup.Values.Select(s => s.Id).ToList();
            var existingAttendances = await _context.Attendances
                .Where(a => studentIds.Contains(a.StudentId) && sessionIds.Contains(a.HalqeSessionId))
                .ToListAsync();

            var attendanceLookup = existingAttendances.ToDictionary(a => (a.StudentId, a.HalqeSessionId));

            var newAttendances = new List<Attendance>();
            var inserted = 0;

            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                foreach (var entry in entries)
                {
                    if (!studentLookup.TryGetValue(entry.StudentId, out var student))
                        continue;

                    if (student.SaturdayHalqeId == Guid.Empty)
                        continue;

                    var session = sessionLookup[entry.Date.Date];
                    if (!lessonLookup.TryGetValue(student.SaturdayHalqeId, out var lesson))
                        continue;

                    var key = (student.Id, session.Id);

                    if (attendanceLookup.TryGetValue(key, out var existing))
                    {
                        if (existing.TeacherId != teacherId.Value)
                            return GeneralResponse.Unauthorized("لا تملك صلاحية تعديل هذا السجل.");

                        existing.Status = entry.Status;
                        existing.Note = entry.Note;
                        continue;
                    }

                    var attendance = new Attendance
                    {
                        Id = Guid.NewGuid(),
                        StudentId = student.Id,
                        TeacherId = teacherId.Value,
                        HalqeSessionId = session.Id,
                        SaturdayLessonId = lesson.Id,
                        Status = entry.Status,
                        Note = entry.Note
                    };

                    newAttendances.Add(attendance);
                    attendanceLookup[key] = attendance;
                    inserted++;
                }

                if (newSessions.Count > 0)
                    await _context.HalqaSessions.AddRangeAsync(newSessions);

                if (newLessons.Count > 0)
                    await _context.Set<SaturdayLesson>().AddRangeAsync(newLessons);

                if (newAttendances.Count > 0)
                    await _context.Attendances.AddRangeAsync(newAttendances);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                return GeneralResponse.InternalError("حدث خطأ أثناء تسجيل الحضور.");
            }

            return GeneralResponse.Ok("تم تسجيل الحضور.", new RecordDailyAttendanceResponse
            {
                Success = true,
                Message = "تم تسجيل الحضور.",
                RecordsInserted = inserted
            });
        }

        public async Task<GeneralResponse> RecordAbsenceAsync(RecordAbsenceRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var entry = new AttendanceEntry
            {
                StudentId = request.StudentId,
                Date = DateTime.UtcNow.Date,
                Status = request.WithExcuse ? AttendanceStatus.Excused : AttendanceStatus.Absent,
                Note = request.Note
            };

            return await RecordDailyAttendanceAsync(new RecordDailyAttendanceRequest
            {
                Entries = new List<AttendanceEntry> { entry }
            });
        }

        public async Task<GeneralResponse> ModifyAttendanceRecordAsync(ModifyAttendanceRecordRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var record = await _context.Attendances.FindAsync(request.RecordId);
            if (record == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || record.TeacherId != teacherId.Value)
                return GeneralResponse.Unauthorized("لا تملك صلاحية تعديل هذا السجل.");

            record.Status = request.NewStatus;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تعديل السجل.", new ModifyAttendanceRecordResponse
            {
                Success = true,
                Message = "تم التعديل."
            });
        }

        public async Task<GeneralResponse> DeleteAttendanceRecordAsync(Guid recordId)
        {
            var record = await _context.Attendances.FindAsync(recordId);
            if (record == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || record.TeacherId != teacherId.Value)
                return GeneralResponse.Unauthorized("لا تملك صلاحية حذف هذا السجل.");

            _context.Attendances.Remove(record);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف السجل.");
        }

        public async Task<GeneralResponse> UpdateAttendanceStatusAsync(Guid recordId, UpdateAttendanceStatusRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var record = await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.HalqeSession)
                .FirstOrDefaultAsync(a => a.Id == recordId);

            if (record == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            var teacherId = _currentUserService.CurrentUserId;
            if (!teacherId.HasValue || record.TeacherId != teacherId.Value)
                return GeneralResponse.Unauthorized("لا تملك صلاحية تعديل هذا السجل.");

            record.Status = request.NewStatus;
            record.Note = request.Note;

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تحديث الحالة.", MapAttendanceRecord(record));
        }

        public async Task<GeneralResponse> CalculateAttendanceRateAsync(CalculateAttendanceRateRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var query = _context.Attendances.AsNoTracking().Where(a => a.StudentId == request.StudentId);

            var total = await query.CountAsync();
            var present = await query.CountAsync(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);

            var rate = total == 0 ? 0 : Math.Round((double)present / total * 100, 2);

            return GeneralResponse.Ok("تم حساب نسبة الحضور.", new CalculateAttendanceRateResponse { Rate = rate });
        }

        public async Task<GeneralResponse> MonitorFrequentAbsencesAsync(MonitorFrequentAbsencesRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.Student)
                .Where(a => a.Status == AttendanceStatus.Absent);

            var students = await query
                .GroupBy(a => a.StudentId)
                .Where(g => g.Count() >= request.Threshold)
                .Select(g => new AttendanceStudentDto
                {
                    Id = g.Key,
                    Name = g.First().Student.name,
                    Email = g.First().Student.Email,
                    Phone = g.First().Student.PhoneNumber
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الغياب المتكرر.", new MonitorFrequentAbsencesResponse { Students = students });
        }

        public async Task<GeneralResponse> ExportAttendanceAsync(ExportAttendanceRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var records = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.Student)
                .Include(a => a.HalqeSession)
                .Where(a => a.HalqeSession.HalqaId == request.CircleId)
                .OrderByDescending(a => a.HalqeSession.date)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("StudentName,Date,Status,Note");

            foreach (var r in records)
            {
                var line = string.Join(",",
                    EscapeCsv(r.Student?.name),
                    EscapeCsv(r.HalqeSession?.date.ToString("yyyy-MM-dd")),
                    EscapeCsv(r.Status.ToString()),
                    EscapeCsv(r.Note));
                sb.AppendLine(line);
            }

            return GeneralResponse.Ok("تم تصدير السجلات.", new ExportAttendanceResponse
            {
                FileContent = Encoding.UTF8.GetBytes(sb.ToString()),
                ContentType = "text/csv",
                FileName = "attendance.csv"
            });
        }

        public async Task<GeneralResponse> GetAttendanceRecordByIdAsync(Guid recordId)
        {
            var record = await _context.Attendances
                .AsNoTracking()
                .Include(a => a.Student)
                .Include(a => a.HalqeSession)
                .FirstOrDefaultAsync(a => a.Id == recordId);

            if (record == null)
                return GeneralResponse.NotFound("السجل غير موجود.");

            return GeneralResponse.Ok("تم جلب السجل.", MapAttendanceRecord(record));
        }

        public async Task<GeneralResponse> GetAllAttendanceRecordsAsync(AttendanceRecordFilter filter)
        {
            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.Student)
                .Include(a => a.HalqeSession)
                .AsQueryable();

            if (filter.StudentId.HasValue)
                query = query.Where(a => a.StudentId == filter.StudentId.Value);

            if (filter.CircleId.HasValue)
                query = query.Where(a => a.HalqeSession.HalqaId == filter.CircleId.Value);

            if (filter.Status.HasValue)
                query = query.Where(a => a.Status == filter.Status.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.HalqeSession.date >= filter.FromDate.Value.Date);

            if (filter.ToDate.HasValue)
                query = query.Where(a => a.HalqeSession.date <= filter.ToDate.Value.Date);

            var total = await query.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var items = await query
                .OrderByDescending(a => a.HalqeSession.date)
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(a => new AttendanceRecordSummaryDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentName = a.Student.name,
                    Date = a.HalqeSession.date,
                    Status = a.Status
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب السجلات.", items, filter.PageNumber, filter.PageSize, total);
        }

        public async Task<GeneralResponse> GetStudentAttendanceRateAsync(Guid studentId, DateTime fromDate, DateTime toDate)
        {
            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.HalqeSession)
                .Where(a => a.StudentId == studentId && a.HalqeSession.date >= fromDate.Date && a.HalqeSession.date <= toDate.Date);

            var total = await query.CountAsync();
            var present = await query.CountAsync(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
            var absent = await query.CountAsync(a => a.Status == AttendanceStatus.Absent || a.Status == AttendanceStatus.Excused);

            var rate = total == 0 ? 0 : Math.Round((double)present / total * 100, 2);

            return GeneralResponse.Ok("تم حساب نسبة الحضور.", new AttendanceRateDto
            {
                StudentId = studentId,
                FromDate = fromDate.Date,
                ToDate = toDate.Date,
                TotalSessions = total,
                PresentCount = present,
                AbsentCount = absent,
                Rate = rate
            });
        }

        public async Task<GeneralResponse> GetFrequentAbsencesAsync(FrequentAbsencesFilter filter)
        {
            var query = _context.Attendances
                .AsNoTracking()
                .Include(a => a.Student)
                .Include(a => a.HalqeSession)
                .Where(a => a.Status == AttendanceStatus.Absent);

            if (filter.CircleId.HasValue)
                query = query.Where(a => a.HalqeSession.HalqaId == filter.CircleId.Value);

            if (filter.FromDate.HasValue)
                query = query.Where(a => a.HalqeSession.date >= filter.FromDate.Value.Date);

            if (filter.ToDate.HasValue)
                query = query.Where(a => a.HalqeSession.date <= filter.ToDate.Value.Date);

            var grouped = query.GroupBy(a => a.StudentId).Where(g => g.Count() >= filter.Threshold);

            var total = await grouped.CountAsync();
            var skip = (filter.PageNumber - 1) * filter.PageSize;

            var students = await grouped
                .Skip(skip)
                .Take(filter.PageSize)
                .Select(g => new AttendanceStudentDto
                {
                    Id = g.Key,
                    Name = g.First().Student.name,
                    Email = g.First().Student.Email,
                    Phone = g.First().Student.PhoneNumber
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الطلاب كثيري الغياب.", new FrequentAbsencesResultDto
            {
                Threshold = filter.Threshold,
                TotalStudents = total,
                Students = students
            }, filter.PageNumber, filter.PageSize, total);
        }

        private async Task<Guid?> ResolveCircleIdAsync()
        {
            var userId = _currentUserService.CurrentUserId;
            if (!userId.HasValue) return null;

            return await _context.Halqas
                .AsNoTracking()
                .Where(h => h.TeacherId == userId.Value)
                .Select(h => (Guid?)h.Id)
                .FirstOrDefaultAsync();
        }

        private static AttendanceRecordDto MapAttendanceRecord(Attendance record)
        {
            return new AttendanceRecordDto
            {
                Id = record.Id,
                StudentId = record.StudentId,
                StudentName = record.Student?.name ?? string.Empty,
                Date = record.HalqeSession?.date ?? DateTime.UtcNow,
                Status = record.Status,
                Note = record.Note
            };
        }

        private static string EscapeCsv(string? value)
        {
            if (string.IsNullOrEmpty(value))
                return string.Empty;

            var escaped = value.Replace("\"", "\"\"");
            var needsQuotes = escaped.Contains(',') || escaped.Contains('\n') || escaped.Contains('\r');

            return needsQuotes ? $"\"{escaped}\"" : escaped;
        }
    }
}