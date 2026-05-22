
using Microsoft.Extensions.Caching.Memory;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Analytics;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Analytics;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;

        // ثوابت التحليل بدل الأرقام السحرية
        private const double RankWeightExam = 0.50;
        private const double RankWeightAttendance = 0.30;
        private const double RankWeightProgress = 0.20;

        private const double TrendThreshold = 0.5;
        private const int TrendLastDays = 30;
        private const int TrendPreviousDays = 60;

        // لتقليل حجم البيانات المسحوبة
        private const int DefaultAnalyticsWindowDays = 180;

        // كاش
        private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(7);

        // تخزين تقارير دائم مبسط عبر ملف JSON (بديل مؤقت عن DB Table)
        private static readonly SemaphoreSlim ReportFileLock = new(1, 1);
        private static readonly string ReportsFilePath = Path.Combine(AppContext.BaseDirectory, "analytics-reports.json");

        public AnalyticsService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        // نقطة الدخول لتحليل بيانات الطالب (تعمل بشكل غير متزامن)
        public async Task<GeneralResponse> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid student id.");

                var dto = await BuildStudentAnalyticsAsync(request.Id);
                return GeneralResponse.Ok("Student analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze student failed.");
            }
        }

        // تحليل مباشر لبيانات طالب عبر المعرف
        public async Task<GeneralResponse> AnalyzeStudentDataByIdAsync(Guid studentId)
        {
            try
            {
                if (studentId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid student id.");

                var dto = await BuildStudentAnalyticsAsync(studentId);
                return GeneralResponse.Ok("Student analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze student by id failed.");
            }
        }

        // نقطة الدخول لتحليل أداء المعلم
        public async Task<GeneralResponse> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid teacher id.");

                var dto = await BuildTeacherAnalyticsAsync(request.Id);
                return GeneralResponse.Ok("Teacher analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze teacher failed.");
            }
        }

        // تحليل مباشر لأداء معلم عبر المعرف
        public async Task<GeneralResponse> AnalyzeTeacherPerformanceByIdAsync(Guid teacherId)
        {
            try
            {
                if (teacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid teacher id.");

                var dto = await BuildTeacherAnalyticsAsync(teacherId);
                return GeneralResponse.Ok("Teacher analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze teacher by id failed.");
            }
        }

        // تحليل فعالية حلقة
        public async Task<GeneralResponse> AnalyzeHalqaEffectivenessAsync(AnalyzeHalqaEffectivenessRequest request)
        {
            try
            {
                if (request == null || request.Id == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid circle id.");

                var dto = await BuildHalqaAnalyticsAsync(request.Id);
                return GeneralResponse.Ok("Circle analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze circle failed.");
            }
        }

        // تحليل مباشر لفعالية حلقة عبر المعرف
        public async Task<GeneralResponse> AnalyzeHalqaEffectivenessByIdAsync(Guid HalqaId)
        {
            try
            {
                if (HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid halqa id.");

                var dto = await BuildHalqaAnalyticsAsync(HalqaId);
                return GeneralResponse.Ok("Halqa analytics calculated.", dto);
            }
            catch
            {
                return GeneralResponse.InternalError("Analyze halqa by id failed.");
            }
        }

        // حفظ تقرير تحليلي
        public async Task<GeneralResponse> SaveAnalyticsReportAsync(SaveAnalyticsReportRequest request)
        {
            try
            {
                var report = await SaveAnalyticsReportInternalAsync(request);
                return GeneralResponse.Ok("Report saved successfully.", report);
            }
            catch (ArgumentException ex)
            {
                return GeneralResponse.BadRequest(ex.Message);
            }
            catch
            {
                return GeneralResponse.InternalError("Save analytics report failed.");
            }
        }

        // جلب تقرير تحليلي بالمعرف
        public async Task<GeneralResponse> GetAnalyticsReportByIdAsync(Guid reportId)
        {
            try
            {
                var report = await GetAnalyticsReportByIdInternalAsync(reportId);
                return GeneralResponse.Ok("Report retrieved.", report);
            }
            catch (KeyNotFoundException)
            {
                return GeneralResponse.NotFound("Report not found.");
            }
            catch
            {
                return GeneralResponse.InternalError("Get analytics report failed.");
            }
        }

        // جلب قائمة التقارير مع التصفية والتصفح
        public async Task<GeneralResponse> GetAllAnalyticsReportsAsync(AnalyticsReportFilter filter)
        {
            try
            {
                var paged = await GetAllAnalyticsReportsInternalAsync(filter);
                return GeneralResponse.Ok("Reports retrieved.", paged);
            }
            catch
            {
                return GeneralResponse.InternalError("Get analytics reports failed.");
            }
        }

        // تحديث تقرير تحليلي
        public async Task<GeneralResponse> UpdateAnalyticsReportAsync(Guid reportId, UpdateAnalyticsReportRequest request)
        {
            try
            {
                var report = await UpdateAnalyticsReportInternalAsync(reportId, request);
                return GeneralResponse.Ok("Report updated.", report);
            }
            catch (KeyNotFoundException)
            {
                return GeneralResponse.NotFound("Report not found.");
            }
            catch (ArgumentException ex)
            {
                return GeneralResponse.BadRequest(ex.Message);
            }
            catch
            {
                return GeneralResponse.InternalError("Update analytics report failed.");
            }
        }

        // حذف تقرير تحليلي
        public async Task<GeneralResponse> DeleteAnalyticsReportAsync(Guid reportId)
        {
            try
            {
                var removed = await DeleteAnalyticsReportInternalAsync(reportId);
                if (!removed)
                    return GeneralResponse.NotFound("Report not found.");

                return GeneralResponse.Ok("Report deleted.", removed);
            }
            catch
            {
                return GeneralResponse.InternalError("Delete analytics report failed.");
            }
        }

        // ========================= Internal analytics builders =========================

        private async Task<StudentAnalyticsDto> BuildStudentAnalyticsAsync(Guid studentId)
        {
            var cacheKey = $"analytics:student:{studentId}";
            if (_cache.TryGetValue(cacheKey, out StudentAnalyticsDto cachedStudent))
                return cachedStudent;

            var student = await _unitOfWork.Repository<Student>().GetByIdAsync(studentId);
            if (student is null)
            {
                return new StudentAnalyticsDto
                {
                    StudentId = studentId,
                    StudentName = "Unknown",
                    Gender = string.Empty,
                    MonthlyProgress = new List<MonthlyProgress>(),
                    PerformanceTrend = "Stable"
                };
            }

            var fromDate = DateTime.UtcNow.AddDays(-DefaultAnalyticsWindowDays);

            var progressSpec = Spec.For<ProgressEntry>(p => p.StudentId == studentId && p.Date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec))
                .OrderBy(p => p.Date)
                .ToList();

            var examsSpec = Spec.For<Exam>(e => e.StudentId == studentId && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            var attendanceSpec = Spec.For<Attendance>(a => a.StudentId == studentId);
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            var monthlyProgress = progressEntries
                .GroupBy(p => new { p.Date.Year, p.Date.Month })
                .Select(g =>
                {
                    var monthStart = new DateTime(g.Key.Year, g.Key.Month, 1);
                    var monthAvgScore = exams
                        .Where(e => e.date.Year == g.Key.Year && e.date.Month == g.Key.Month)
                        .Select(e => (double?)e.mark)
                        .Average() ?? 0.0;

                    return new MonthlyProgress
                    {
                        Month = monthStart,
                        PagesMemorized = g.Select(x => x.PageNumber).Distinct().Count(),
                        AverageScore = Math.Round(monthAvgScore, 2)
                    };
                })
                .OrderBy(x => x.Month)
                .ToList();

            var result = new StudentAnalyticsDto
            {
                StudentId = student.Id,
                StudentName = student.name ?? string.Empty,
                Age = student.age,
                Gender = student.gender ?? string.Empty,
                EnrollmentDate = student.EnrollmentDate,
                TotalSessions = attendances.Count,
                TotalMemorizedPages = progressEntries.Select(p => p.PageNumber).Distinct().Count(),
                LastMemorizedPage = progressEntries.Select(p => (int?)p.PageNumber).Max() ?? 0,
                AverageExamScore = Math.Round(exams.Select(e => (double?)e.mark).Average() ?? 0.0, 2),
                TotalPoints = exams.Sum(e => e.mark),
                MonthlyProgress = monthlyProgress,
                PerformanceTrend = CalculateStudentTrend(progressEntries)
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }

        private async Task<TeacherAnalyticsDto> BuildTeacherAnalyticsAsync(Guid teacherId)
        {
            var cacheKey = $"analytics:teacher:{teacherId}";
            if (_cache.TryGetValue(cacheKey, out TeacherAnalyticsDto cachedTeacher))
                return cachedTeacher;

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(teacherId);
            if (teacher is null)
            {
                return new TeacherAnalyticsDto
                {
                    TeacherId = teacherId,
                    TeacherName = "Unknown",
                    HalaqasPerformance = new List<HalaqaPerformance>()
                };
            }

            var fromDate = DateTime.UtcNow.AddDays(-DefaultAnalyticsWindowDays);

            var halqaSpec = Spec.For<Halqa>(h => h.TeacherId == teacherId);
            var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(halqaSpec)).ToList();
            var halqaIds = halqas.Select(h => h.Id).ToHashSet();

            if (halqaIds.Count == 0)
            {
                var emptyResult = new TeacherAnalyticsDto
                {
                    TeacherId = teacher.Id,
                    TeacherName = teacher.name ?? string.Empty,
                    AssignedHalaqasCount = 0,
                    TotalStudentsCount = 0,
                    AverageStudentAttendance = 0,
                    AverageStudentProgress = 0,
                    AverageExamScore = 0,
                    TotalPointsEarnedByStudents = 0,
                    HalaqasPerformance = new List<HalaqaPerformance>()
                };

                _cache.Set(cacheKey, emptyResult, CacheDuration);
                return emptyResult;
            }

            var progressSpec = Spec.For<ProgressEntry>(p => halqaIds.Contains(p.HalqaId) && p.Date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

            var sessionSpec = Spec.For<HalqaSession>(s => halqaIds.Contains(s.HalqaId) && s.date >= fromDate);
            var sessions = (await _unitOfWork.Repository<HalqaSession>().GetAllAsync(sessionSpec)).ToList();
            var sessionIds = sessions.Select(s => s.Id).ToHashSet();

            var attendanceSpec = Spec.For<Attendance>(a => sessionIds.Contains(a.HalqeSessionId));
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            var examsSpec = Spec.For<Exam>(e => e.TeacherId == teacherId && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            var progressByHalqa = progressEntries
                .GroupBy(p => p.HalqaId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var sessionsByHalqa = sessions
                .GroupBy(s => s.HalqaId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToHashSet());

            var attendanceBySession = attendances
                .GroupBy(a => a.HalqeSessionId)
                .ToDictionary(g => g.Key, g => g.Count());

            var studentIds = progressEntries.Select(p => p.StudentId).Distinct().ToList();

            var avgAttendance = (studentIds.Count == 0 || sessions.Count == 0)
                ? 0.0
                : (double)attendances.Count / (studentIds.Count * sessions.Count) * 100.0;

            var avgProgress = progressEntries.Select(p => (double?)p.PageNumber).Average() ?? 0.0;

            var halqasPerformance = new List<HalaqaPerformance>();

            foreach (var halqa in halqas)
            {
                var hProgress = progressByHalqa.TryGetValue(halqa.Id, out var p) ? p : new List<ProgressEntry>();
                var hStudents = hProgress.Select(x => x.StudentId).Distinct().ToList();

                var hSessionIds = sessionsByHalqa.TryGetValue(halqa.Id, out var hs)
                    ? hs
                    : new HashSet<Guid>();

                var hAttendanceCount = hSessionIds.Sum(id =>
                    attendanceBySession.TryGetValue(id, out var c) ? c : 0);

                var hAvgAttendance = (hStudents.Count == 0 || hSessionIds.Count == 0)
                    ? 0.0
                    : (double)hAttendanceCount / (hStudents.Count * hSessionIds.Count) * 100.0;

                var hAvgProgress = hProgress.Select(x => (double?)x.PageNumber).Average() ?? 0.0;

                halqasPerformance.Add(new HalaqaPerformance
                {
                    HalaqaId = halqa.Id,
                    HalaqaName = halqa.Name ?? string.Empty,
                    StudentsCount = hStudents.Count,
                    AverageAttendance = Math.Round(hAvgAttendance, 2),
                    AverageProgress = Math.Round(hAvgProgress, 2)
                });
            }

            var result = new TeacherAnalyticsDto
            {
                TeacherId = teacher.Id,
                TeacherName = teacher.name ?? string.Empty,
                AssignedHalaqasCount = halqas.Count,
                TotalStudentsCount = studentIds.Count,
                AverageStudentAttendance = Math.Round(avgAttendance, 2),
                AverageStudentProgress = Math.Round(avgProgress, 2),
                AverageExamScore = Math.Round(exams.Select(e => (double?)e.mark).Average() ?? 0.0, 2),
                TotalPointsEarnedByStudents = exams.Sum(e => e.mark),
                HalaqasPerformance = halqasPerformance
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }

        private async Task<HalqaAnalyticsDto> BuildHalqaAnalyticsAsync(Guid HalqaId)
        {
            var cacheKey = $"analytics:halqa:{HalqaId}";
            if (_cache.TryGetValue(cacheKey, out HalqaAnalyticsDto cachedHalqa))
                return cachedHalqa;

            var Halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(HalqaId);
            if (Halqa is null)
            {
                return new HalqaAnalyticsDto
                {
                    HalqaId = HalqaId,
                    HalqaName = "Unknown",
                    TopPerformers = new List<StudentPerformance>(),
                    StrugglingStudents = new List<StudentPerformance>()
                };
            }

            var fromDate = DateTime.UtcNow.AddDays(-DefaultAnalyticsWindowDays);

            var progressSpec = Spec.For<ProgressEntry>(p => p.HalqaId == HalqaId && p.Date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

            var sessionSpec = Spec.For<HalqaSession>(s => s.HalqaId == HalqaId && s.date >= fromDate);
            var sessions = (await _unitOfWork.Repository<HalqaSession>().GetAllAsync(sessionSpec)).ToList();
            var sessionIds = sessions.Select(s => s.Id).ToHashSet();

            var attendanceSpec = Spec.For<Attendance>(a => sessionIds.Contains(a.HalqeSessionId));
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            var studentIds = progressEntries.Select(p => p.StudentId)
                .Concat(attendances.Select(a => a.StudentId))
                .Distinct()
                .ToList();

            var studentIdSet = studentIds.ToHashSet();

            var examsSpec = Spec.For<Exam>(e => studentIdSet.Contains(e.StudentId) && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            var studentSpec = Spec.For<Student>(s => studentIdSet.Contains(s.Id));
            var students = (await _unitOfWork.Repository<Student>().GetAllAsync(studentSpec))
                .ToDictionary(s => s.Id, s => s);

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync((Guid)Halqa.TeacherId);

            var avgAttendanceRate = (studentIds.Count == 0 || sessionIds.Count == 0)
                ? 0.0
                : (double)attendances.Count / (studentIds.Count * sessionIds.Count) * 100.0;

            var avgProgress = progressEntries.Select(p => (double?)p.PageNumber).Average() ?? 0.0;

            var activeStudentsCount = progressEntries
                .Where(p => p.Date >= DateTime.UtcNow.AddDays(-TrendLastDays))
                .Select(p => p.StudentId)
                .Distinct()
                .Count();

            var retentionRate = studentIds.Count == 0
                ? 0.0
                : (double)activeStudentsCount / studentIds.Count * 100.0;

            var progressByStudent = progressEntries
                .GroupBy(p => p.StudentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var examsByStudent = exams
                .GroupBy(e => e.StudentId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var attendanceByStudent = attendances
                .GroupBy(a => a.StudentId)
                .ToDictionary(g => g.Key, g => g.Count());

            var ranked = new List<(StudentPerformance Data, double RankScore)>();

            foreach (var sid in studentIds)
            {
                var sp = progressByStudent.TryGetValue(sid, out var plist) ? plist : new List<ProgressEntry>();
                var se = examsByStudent.TryGetValue(sid, out var elist) ? elist : new List<Exam>();
                var sa = attendanceByStudent.TryGetValue(sid, out var ac) ? ac : 0;

                var avgScore = se.Select(x => (double?)x.mark).Average() ?? 0.0;
                var attRate = sessionIds.Count == 0 ? 0.0 : (double)sa / sessionIds.Count * 100.0;
                var pages = sp.Select(x => x.PageNumber).Distinct().Count();

                var dto = new StudentPerformance
                {
                    StudentId = sid,
                    StudentName = students.TryGetValue(sid, out var st) ? (st.name ?? string.Empty) : string.Empty,
                    AttendanceRate = Math.Round(attRate, 2),
                    PagesMemorized = pages,
                    AverageScore = Math.Round(avgScore, 2)
                };

                var rankScore = (avgScore * RankWeightExam) +
                                (attRate * RankWeightAttendance) +
                                (pages * RankWeightProgress);

                ranked.Add((dto, rankScore));
            }

            var result = new HalqaAnalyticsDto
            {
                HalqaId = Halqa.Id,
                HalqaName = Halqa.Name ?? string.Empty,
                HalqaType = Halqa.Type,
                TeacherId = (Guid)Halqa.TeacherId,
                TeacherName = teacher?.name ?? string.Empty,
                StudentsCount = studentIds.Count,
                ActiveStudentsCount = activeStudentsCount,
                AverageAttendanceRate = Math.Round(avgAttendanceRate, 2),
                AverageMemorizationProgress = Math.Round(avgProgress, 2),
                AverageExamScore = Math.Round(exams.Select(e => (double?)e.mark).Average() ?? 0.0, 2),
                RetentionRate = Math.Round(retentionRate, 2),
                TopPerformers = ranked.OrderByDescending(x => x.RankScore).Take(5).Select(x => x.Data).ToList(),
                StrugglingStudents = ranked.OrderBy(x => x.RankScore).Take(5).Select(x => x.Data).ToList()
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }

        // ========================= Reports =========================

        private async Task<AnalyticsReportDto> SaveAnalyticsReportInternalAsync(SaveAnalyticsReportRequest request)
        {
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.ReportType))
                throw new ArgumentException("ReportType is required.", nameof(request));
            if (string.IsNullOrWhiteSpace(request.PayloadJson))
                throw new ArgumentException("PayloadJson is required.", nameof(request));

            var reports = await LoadReportsAsync();

            var report = new AnalyticsReportDto
            {
                Id = Guid.NewGuid(),
                ReportType = request.ReportType.Trim(),
                Title = request.Title?.Trim(),
                PayloadJson = request.PayloadJson,
                StudentId = request.StudentId,
                TeacherId = request.TeacherId,
                HalqId = request.HalqaId,
                CreatedAt = DateTime.UtcNow
            };

            reports.Add(report);
            await SaveReportsAsync(reports);

            return report;
        }

        private async Task<AnalyticsReportDto> GetAnalyticsReportByIdInternalAsync(Guid reportId)
        {
            ValidateGuid(reportId, nameof(reportId));

            var reports = await LoadReportsAsync();
            var report = reports.FirstOrDefault(r => r.Id == reportId);

            if (report is null)
                throw new KeyNotFoundException($"Analytics report '{reportId}' not found.");

            return report;
        }

        private async Task<PagedResult<AnalyticsReportSummaryDto>> GetAllAnalyticsReportsInternalAsync(AnalyticsReportFilter filter)
        {
            filter ??= new AnalyticsReportFilter();

            var query = (await LoadReportsAsync()).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(filter.ReportType))
                query = query.Where(r => r.ReportType.Equals(filter.ReportType, StringComparison.OrdinalIgnoreCase));

            if (filter.StudentId.HasValue)
                query = query.Where(r => r.StudentId == filter.StudentId);

            if (filter.TeacherId.HasValue)
                query = query.Where(r => r.TeacherId == filter.TeacherId);

            if (filter.HalqId.HasValue)
                query = query.Where(r => r.HalqId == filter.HalqId);

            if (filter.From.HasValue)
                query = query.Where(r => r.CreatedAt >= filter.From.Value);

            if (filter.To.HasValue)
                query = query.Where(r => r.CreatedAt <= filter.To.Value);

            query = query.OrderByDescending(r => r.CreatedAt);

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

            var totalCount = query.Count();

            var items = query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(r => new AnalyticsReportSummaryDto
                {
                    Id = r.Id,
                    ReportType = r.ReportType,
                    Title = r.Title,
                    StudentId = r.StudentId,
                    TeacherId = r.TeacherId,
                    HalqId = r.HalqId,
                    CreatedAt = r.CreatedAt
                })
                .ToList();

            return new PagedResult<AnalyticsReportSummaryDto>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        private async Task<AnalyticsReportDto> UpdateAnalyticsReportInternalAsync(Guid reportId, UpdateAnalyticsReportRequest request)
        {
            ValidateGuid(reportId, nameof(reportId));
            if (request is null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.PayloadJson))
                throw new ArgumentException("PayloadJson is required.", nameof(request));

            var reports = await LoadReportsAsync();
            var report = reports.FirstOrDefault(r => r.Id == reportId);

            if (report is null)
                throw new KeyNotFoundException($"Analytics report '{reportId}' not found.");

            report.Title = request.Title?.Trim();
            report.PayloadJson = request.PayloadJson;
            report.UpdatedAt = DateTime.UtcNow;

            await SaveReportsAsync(reports);
            return report;
        }

        private async Task<bool> DeleteAnalyticsReportInternalAsync(Guid reportId)
        {
            ValidateGuid(reportId, nameof(reportId));

            var reports = await LoadReportsAsync();
            var removed = reports.RemoveAll(r => r.Id == reportId) > 0;

            if (removed)
                await SaveReportsAsync(reports);

            return removed;
        }

        // ========================= Helpers =========================

        private static string CalculateStudentTrend(List<ProgressEntry> entries)
        {
            if (entries.Count < 2) return "Stable";

            var now = DateTime.UtcNow;

            var lastAvg = entries
                .Where(p => p.Date >= now.AddDays(-TrendLastDays))
                .Select(p => (double)p.LevelScore)
                .DefaultIfEmpty(0)
                .Average();

            var prevAvg = entries
                .Where(p => p.Date >= now.AddDays(-TrendPreviousDays) && p.Date < now.AddDays(-TrendLastDays))
                .Select(p => (double)p.LevelScore)
                .DefaultIfEmpty(0)
                .Average();

            if (lastAvg > prevAvg + TrendThreshold) return "Improving";
            if (lastAvg < prevAvg - TrendThreshold) return "Declining";
            return "Stable";
        }

        private static void ValidateGuid(Guid value, string paramName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Invalid Guid value.", paramName);
        }

        private static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };

        private static async Task<List<AnalyticsReportDto>> LoadReportsAsync()
        {
            await ReportFileLock.WaitAsync();
            try
            {
                if (!File.Exists(ReportsFilePath))
                    return new List<AnalyticsReportDto>();

                var json = await File.ReadAllTextAsync(ReportsFilePath);
                if (string.IsNullOrWhiteSpace(json))
                    return new List<AnalyticsReportDto>();

                var data = JsonSerializer.Deserialize<List<AnalyticsReportDto>>(json, JsonOptions);
                return data ?? new List<AnalyticsReportDto>();
            }
            finally
            {
                ReportFileLock.Release();
            }
        }

        private static async Task SaveReportsAsync(List<AnalyticsReportDto> reports)
        {
            await ReportFileLock.WaitAsync();
            try
            {
                var json = JsonSerializer.Serialize(reports, JsonOptions);
                await File.WriteAllTextAsync(ReportsFilePath, json);
            }
            finally
            {
                ReportFileLock.Release();
            }
        }
        public async Task<GeneralResponse> GetTeacherDashboardStatisticsAsync(GetTeacherDashboardStatisticsRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid teacher id.");

                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher not found.");

                var fromDate = (request.FromDate ?? DateTime.UtcNow.AddDays(-30)).Date;
                var toDate = (request.ToDate ?? DateTime.UtcNow).Date;

                if (fromDate > toDate)
                    return GeneralResponse.BadRequest("Invalid date range.");

                var halqaSpec = Spec.For<Halqa>(h => h.TeacherId == request.TeacherId);
                var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(halqaSpec)).ToList();
                var halqaIds = halqas.Select(h => h.Id).ToHashSet();

                if (halqaIds.Count == 0)
                {
                    return GeneralResponse.Ok("Teacher dashboard stats.", new TeacherDashboardStatisticsDto
                    {
                        TeacherId = teacher.Id,
                        TeacherName = teacher.name ?? string.Empty
                    });
                }

                var progressSpec = Spec.For<ProgressEntry>(p => halqaIds.Contains(p.HalqaId) && p.Date >= fromDate && p.Date <= toDate);
                var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

                var studentIds = progressEntries.Select(p => p.StudentId).Distinct().ToList();
                var studentsSpec = Spec.For<Student>(s => studentIds.Contains(s.Id));
                var students = (await _unitOfWork.Repository<Student>().GetAllAsync(studentsSpec)).ToList();

                var perStudentLastPage = progressEntries
                    .GroupBy(p => p.StudentId)
                    .Select(g => g.Max(x => x.PageNumber))
                    .DefaultIfEmpty(0)
                    .Average();

                var memorizationRate = studentIds.Count == 0
                    ? 0.0
                    : Math.Round(perStudentLastPage / 604d * 100d, 2);

                var sessionSpec = Spec.For<HalqaSession>(s => halqaIds.Contains(s.HalqaId) && s.date >= fromDate && s.date <= toDate);
                var sessions = (await _unitOfWork.Repository<HalqaSession>().GetAllAsync(sessionSpec)).ToList();
                var sessionIds = sessions.Select(s => s.Id).ToHashSet();

                var attendanceSpec = Spec.For<Attendance>(a => sessionIds.Contains(a.HalqeSessionId));
                var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

                var totalAttendance = attendances.Count;
                var presentAttendance = attendances.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
                var attendanceRate = totalAttendance == 0 ? 0 : Math.Round((double)presentAttendance / totalAttendance * 100, 2);

                var totalPoints = students.Sum(s => s.score);

                return GeneralResponse.Ok("Teacher dashboard stats.", new TeacherDashboardStatisticsDto
                {
                    TeacherId = teacher.Id,
                    TeacherName = teacher.name ?? string.Empty,
                    StudentsCount = studentIds.Count,
                    HalaqasCount = halqas.Count,
                    MemorizationRate = memorizationRate,
                    AttendanceRate = attendanceRate,
                    TotalPoints = totalPoints
                });
            }
            catch
            {
                return GeneralResponse.InternalError("Get teacher dashboard stats failed.");
            }
        }

        public async Task<GeneralResponse> GetMostRegressingStudentAsync(GetMostRegressingStudentRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid teacher id.");

                var halqaSpec = Spec.For<Halqa>(h => h.TeacherId == request.TeacherId);
                var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(halqaSpec)).ToList();
                var halqaIds = halqas.Select(h => h.Id).ToHashSet();

                if (halqaIds.Count == 0)
                    return GeneralResponse.NotFound("No halqas found.");

                var recentTo = (request.RecentTo ?? DateTime.UtcNow).Date;
                var recentFrom = (request.RecentFrom ?? recentTo.AddDays(-request.WindowDays)).Date;
                var prevFrom = recentFrom.AddDays(-request.WindowDays);
                var prevTo = recentFrom.AddDays(-1);

                var progressSpec = Spec.For<ProgressEntry>(p => halqaIds.Contains(p.HalqaId) && p.Date >= prevFrom && p.Date <= recentTo);
                var progress = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

                var recent = progress
                    .Where(p => p.Date >= recentFrom && p.Date <= recentTo)
                    .GroupBy(p => p.StudentId)
                    .ToDictionary(g => g.Key, g => g.Average(x => (double)x.PageNumber));

                var previous = progress
                    .Where(p => p.Date >= prevFrom && p.Date <= prevTo)
                    .GroupBy(p => p.StudentId)
                    .ToDictionary(g => g.Key, g => g.Average(x => (double)x.PageNumber));

                var allIds = recent.Keys.Union(previous.Keys).ToList();
                if (allIds.Count == 0)
                    return GeneralResponse.NotFound("No progress data found.");

                var minDelta = double.MaxValue;
                Guid targetId = Guid.Empty;
                double prevVal = 0;
                double recentVal = 0;

                foreach (var id in allIds)
                {
                    var r = recent.TryGetValue(id, out var rv) ? rv : 0;
                    var p = previous.TryGetValue(id, out var pv) ? pv : 0;
                    var delta = r - p;

                    if (delta < minDelta)
                    {
                        minDelta = delta;
                        targetId = id;
                        prevVal = p;
                        recentVal = r;
                    }
                }

                var student = await _unitOfWork.Repository<Student>().GetByIdAsync(targetId);

                return GeneralResponse.Ok("Most regressing student.", new RegressingStudentDto
                {
                    StudentId = targetId,
                    StudentName = student?.name ?? string.Empty,
                    PreviousAveragePages = Math.Round(prevVal, 2),
                    RecentAveragePages = Math.Round(recentVal, 2),
                    Delta = Math.Round(minDelta, 2)
                });
            }
            catch
            {
                return GeneralResponse.InternalError("Get most regressing student failed.");
            }
        }

        public async Task<GeneralResponse> GetMotivationPlansSummaryAsync(GetMotivationPlansSummaryRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("Invalid teacher id.");

                var planSize = request.PlanSize <= 0 ? 5 : request.PlanSize;

                var halqaSpec = Spec.For<Halqa>(h => h.TeacherId == request.TeacherId);
                var halqas = (await _unitOfWork.Repository<Halqa>().GetAllAsync(halqaSpec)).ToList();
                var halqaIds = halqas.Select(h => h.Id).ToHashSet();

                if (halqaIds.Count == 0)
                    return GeneralResponse.Ok("Motivation plans summary.", new MotivationPlansSummaryDto());

                var progressSpec = Spec.For<ProgressEntry>(p => halqaIds.Contains(p.HalqaId));
                var progress = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();
                var studentIds = progress.Select(p => p.StudentId).Distinct().ToList();

                var studentsSpec = Spec.For<Student>(s => studentIds.Contains(s.Id));
                var students = (await _unitOfWork.Repository<Student>().GetAllAsync(studentsSpec))
                    .OrderByDescending(s => s.score)
                    .ToList();

                var planOne = students.Take(planSize).ToList();
                var planTwo = students.Skip(planSize).Take(planSize).ToList();

                return GeneralResponse.Ok("Motivation plans summary.", new MotivationPlansSummaryDto
                {
                    PlanOneStudentsCount = planOne.Count,
                    PlanTwoStudentsCount = planTwo.Count,
                    PlanOneAverageScore = planOne.Count == 0 ? 0 : Math.Round(planOne.Average(s => s.score), 2),
                    PlanTwoAverageScore = planTwo.Count == 0 ? 0 : Math.Round(planTwo.Average(s => s.score), 2)
                });
            }
            catch
            {
                return GeneralResponse.InternalError("Get motivation plans summary failed.");
            }
        }
    }
}
