using Microsoft.Extensions.Caching.Memory;
using Moeen.Api.Core.Contracts;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Api.Shared.Requests.Analytics;
using Moeen.Api.Shared.Responses.Analytics;
using System.Text.Json;

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
        public Task<StudentAnalyticsDto> AnalyzeStudentDataAsync(AnalyzeStudentDataRequest request)
        {
            ValidateGuid(request?.Id ?? Guid.Empty, nameof(request.Id));
            return AnalyzeStudentDataByIdAsync(request!.Id);
        }

        // 📊 الدالة الرئيسية لتحليل أداء الطالب: تتحقق من الكاش أولاً، فإن لم تجد تجلب البيانات المصفاة (تقدم/امتحانات/حضور) من القاعدة، تعالجها شهرياً، تبني تقريراً شاملاً، تخزنه مؤقتاً لمدة 7 دقائق، وتُرجعه جاهزاً للعرض

        public async Task<StudentAnalyticsDto> AnalyzeStudentDataByIdAsync(Guid studentId)
        {
            ValidateGuid(studentId, nameof(studentId));

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

            // جلب تقدم الطالب ضمن نافذة زمنية فقط (تحسين أداء)
            var progressSpec = Spec.For<ProgressEntry>(p => p.studentId == studentId && p.date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec))
                .OrderBy(p => p.date)
                .ToList();

            // جلب اختبارات الطالب ضمن نافذة زمنية
            var examsSpec = Spec.For<Exam>(e => e.StudentId == studentId && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            // الحضور (لا يوجد تاريخ مباشر على Attendance في النموذج الحالي)
            var attendanceSpec = Spec.For<Attendance>(a => a.StudentId == studentId);
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            // تجميع شهري
            var monthlyProgress = progressEntries
                .GroupBy(p => new { p.date.Year, p.date.Month })
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
                        PagesMemorized = g.Select(x => x.page_number).Distinct().Count(),
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
                EnrollmentDate = student.enrollmrnt_date,
                TotalSessions = attendances.Count,
                TotalMemorizedPages = progressEntries.Select(p => p.page_number).Distinct().Count(),
                LastMemorizedPage = progressEntries.Select(p => (int?)p.page_number).Max() ?? 0,
                AverageExamScore = Math.Round(exams.Select(e => (double?)e.mark).Average() ?? 0.0, 2),
                TotalPoints = exams.Sum(e => e.mark),
                MonthlyProgress = monthlyProgress,
                PerformanceTrend = CalculateStudentTrend(progressEntries)
            };

            _cache.Set(cacheKey, result, CacheDuration);
            return result;
        }


        // 👨‍🏫 نقطة الدخول لتحليل أداء المعلم: تتحقق من صحة المعرف، ثم تُفوض التنفيذ للمنطق الأساسي مع دعم التنفيذ غير المتزامن

        public Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceAsync(AnalyzeTeacherPerformanceRequest request)
        {
            ValidateGuid(request?.Id ?? Guid.Empty, nameof(request.Id));
            return AnalyzeTeacherPerformanceByIdAsync(request!.Id);
        }


        // 👨‍🏫 يحلل أداء المعلم عبر تجميع بيانات حلقاته (طلاب، تقدم، حضور، جلسات، امتحانات) خلال آخر 180 يوم، مع معالجة مسبقة لتجنب استعلامات N+1، وتخزين النتيجة مؤقتاً لمدة 7 دقائق

        public async Task<TeacherAnalyticsDto> AnalyzeTeacherPerformanceByIdAsync(Guid teacherId)
        {
            ValidateGuid(teacherId, nameof(teacherId));

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

            // حلقات المعلم
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

            // بيانات التقدم ضمن نافذة زمنية
            var progressSpec = Spec.For<ProgressEntry>(p => halqaIds.Contains(p.HalqaId) && p.date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

            // جلسات الحلقة ضمن نافذة زمنية
            var sessionSpec = Spec.For<HalqaSession>(s => halqaIds.Contains(s.HalqaId) && s.date >= fromDate);
            var sessions = (await _unitOfWork.Repository<HalqaSession>().GetAllAsync(sessionSpec)).ToList();
            var sessionIds = sessions.Select(s => s.Id).ToHashSet();

            // حضور الجلسات
            var attendanceSpec = Spec.For<Attendance>(a => sessionIds.Contains(a.HalqeSessionId));
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            // اختبارات المعلم ضمن نافذة زمنية
            var examsSpec = Spec.For<Exam>(e => e.TeacherId == teacherId && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            // تجميعات مسبقة لتجنب N+1
            var progressByHalqa = progressEntries
                .GroupBy(p => p.HalqaId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var sessionsByHalqa = sessions
                .GroupBy(s => s.HalqaId)
                .ToDictionary(g => g.Key, g => g.Select(x => x.Id).ToHashSet());

            var attendanceBySession = attendances
                .GroupBy(a => a.HalqeSessionId)
                .ToDictionary(g => g.Key, g => g.Count());

            var studentIds = progressEntries.Select(p => p.studentId).Distinct().ToList();

            var avgAttendance = (studentIds.Count == 0 || sessions.Count == 0)
                ? 0.0
                : (double)attendances.Count / (studentIds.Count * sessions.Count) * 100.0;

            var avgProgress = progressEntries.Select(p => (double?)p.page_number).Average() ?? 0.0;

            var halqasPerformance = new List<HalaqaPerformance>();

            foreach (var halqa in halqas)
            {
                var hProgress = progressByHalqa.TryGetValue(halqa.Id, out var p) ? p : new List<ProgressEntry>();
                var hStudents = hProgress.Select(x => x.studentId).Distinct().ToList();

                var hSessionIds = sessionsByHalqa.TryGetValue(halqa.Id, out var hs)
                    ? hs
                    : new HashSet<Guid>();

                var hAttendanceCount = hSessionIds.Sum(id =>
                    attendanceBySession.TryGetValue(id, out var c) ? c : 0);

                var hAvgAttendance = (hStudents.Count == 0 || hSessionIds.Count == 0)
                    ? 0.0
                    : (double)hAttendanceCount / (hStudents.Count * hSessionIds.Count) * 100.0;

                var hAvgProgress = hProgress.Select(x => (double?)x.page_number).Average() ?? 0.0;

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

             public Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessAsync(AnalyzeCircleEffectivenessRequest request)
         {
            ValidateGuid(request?.Id ?? Guid.Empty, nameof(request.Id));
            return AnalyzeCircleEffectivenessByIdAsync(request!.Id);
        }



        // ⭕ يحلل فعالية الحلقة التعليمية: يجمع بيانات الطلاب (تقدم، حضور، امتحانات) خلال آخر 180 يوم، يحسب مؤشرات الأداء، يصنف الطلاب حسب الترتيب المرجح، ويحدد أفضل 5 وأضعف 5 طلاب، مع تخزين مؤقت للنتائج

        public async Task<CircleAnalyticsDto> AnalyzeCircleEffectivenessByIdAsync(Guid circleId)
        {
            ValidateGuid(circleId, nameof(circleId));

            var cacheKey = $"analytics:circle:{circleId}";
            if (_cache.TryGetValue(cacheKey, out CircleAnalyticsDto cachedCircle))
                return cachedCircle;

            var circle = await _unitOfWork.Repository<Halqa>().GetByIdAsync(circleId);
            if (circle is null)
            {
                return new CircleAnalyticsDto
                {
                    CircleId = circleId,
                    CircleName = "Unknown",
                    TopPerformers = new List<StudentPerformance>(),
                    StrugglingStudents = new List<StudentPerformance>()
                };
            }

            var fromDate = DateTime.UtcNow.AddDays(-DefaultAnalyticsWindowDays);

            var progressSpec = Spec.For<ProgressEntry>(p => p.HalqaId == circleId && p.date >= fromDate);
            var progressEntries = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec)).ToList();

            var sessionSpec = Spec.For<HalqaSession>(s => s.HalqaId == circleId && s.date >= fromDate);
            var sessions = (await _unitOfWork.Repository<HalqaSession>().GetAllAsync(sessionSpec)).ToList();
            var sessionIds = sessions.Select(s => s.Id).ToHashSet();

            var attendanceSpec = Spec.For<Attendance>(a => sessionIds.Contains(a.HalqeSessionId));
            var attendances = (await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec)).ToList();

            // الطلاب من التقدم + الحضور لتغطية أوسع
            var studentIds = progressEntries.Select(p => p.studentId)
                .Concat(attendances.Select(a => a.StudentId))
                .Distinct()
                .ToList();

            var studentIdSet = studentIds.ToHashSet();

            var examsSpec = Spec.For<Exam>(e => studentIdSet.Contains(e.StudentId) && e.date >= fromDate);
            var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examsSpec)).ToList();

            var studentSpec = Spec.For<Student>(s => studentIdSet.Contains(s.Id));
            var students = (await _unitOfWork.Repository<Student>().GetAllAsync(studentSpec))
                .ToDictionary(s => s.Id, s => s);

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(circle.TeacherId);

            var avgAttendanceRate = (studentIds.Count == 0 || sessionIds.Count == 0)
                ? 0.0
                : (double)attendances.Count / (studentIds.Count * sessionIds.Count) * 100.0;

            var avgProgress = progressEntries.Select(p => (double?)p.page_number).Average() ?? 0.0;

            var activeStudentsCount = progressEntries
                .Where(p => p.date >= DateTime.UtcNow.AddDays(-TrendLastDays))
                .Select(p => p.studentId)
                .Distinct()
                .Count();

            var retentionRate = studentIds.Count == 0
                ? 0.0
                : (double)activeStudentsCount / studentIds.Count * 100.0;

            // تجميعات مسبقة
            var progressByStudent = progressEntries
                .GroupBy(p => p.studentId)
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
                var pages = sp.Select(x => x.page_number).Distinct().Count();

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

            var result = new CircleAnalyticsDto
            {
                CircleId = circle.Id,
                CircleName = circle.Name ?? string.Empty,
                CircleType = circle.type ?? string.Empty,
                TeacherId = circle.TeacherId,
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

        // 💾 يحفظ تقرير تحليلات جديد في ملف JSON: يتحقق من صحة الطلب، يولد معرفاً فريداً وتوقيتاً، يضيف التقرير للقائمة، ويحدث الملف مع ضمان التزامن عبر قفل

        public async Task<AnalyticsReportDto> SaveAnalyticsReportAsync(SaveAnalyticsReportRequest request)
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
                CircleId = request.CircleId,
                CreatedAt = DateTime.UtcNow
            };

            reports.Add(report);
            await SaveReportsAsync(reports);

            return report;
        }

        // 🔍 يجلب تقرير تحليلات محدد حسب المعرف: يتحقق من صحة المعرف، يحمّل التقارير من ملف JSON، يبحث عن التقرير المطلوب، ويرمِ خطأ إذا لم يُوجد
        public async Task<AnalyticsReportDto> GetAnalyticsReportByIdAsync(Guid reportId)
        {
            ValidateGuid(reportId, nameof(reportId));

            var reports = await LoadReportsAsync();
            var report = reports.FirstOrDefault(r => r.Id == reportId);

            if (report is null)
                throw new KeyNotFoundException($"Analytics report '{reportId}' not found.");

            return report;
        }

        // 📋 يجلب قائمة تقارير التحليلات مع دعم الفلترة (نوع، طالب، معلم، حلقة، تاريخ) والترتيب والتقسيم الصفحي، ويُرجع النتائج كموجزات خفيفة مع معلومات الصفحات
        public async Task<PagedResult<AnalyticsReportSummaryDto>> GetAllAnalyticsReportsAsync(AnalyticsReportFilter filter)
        {
            filter ??= new AnalyticsReportFilter();

            var query = (await LoadReportsAsync()).AsEnumerable();

            // فلترة
            if (!string.IsNullOrWhiteSpace(filter.ReportType))
                query = query.Where(r => r.ReportType.Equals(filter.ReportType, StringComparison.OrdinalIgnoreCase));

            if (filter.StudentId.HasValue)
                query = query.Where(r => r.StudentId == filter.StudentId);

            if (filter.TeacherId.HasValue)
                query = query.Where(r => r.TeacherId == filter.TeacherId);

            if (filter.CircleId.HasValue)
                query = query.Where(r => r.CircleId == filter.CircleId);

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
                    CircleId = r.CircleId,
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
        // ✏️ يحدّث تقرير تحليلات موجود: يتحقق من المعرف والطلب، يحمّل التقارير، يبحث عن التقرير، يحدّث حقوله (العنوان، المحتوى، وقت التحديث)، ويحفظ التغييرات في ملف JSON
        public async Task<AnalyticsReportDto> UpdateAnalyticsReportAsync(Guid reportId, UpdateAnalyticsReportRequest request)
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
        // 🗑️ يحذف تقرير تحليلات حسب المعرف: يتحقق من صحة المعرف، يحمّل التقارير، يزيل التقرير المطابق (إن وُجد)، ويحفظ القائمة المحدثة في ملف JSON
        public async Task<bool> DeleteAnalyticsReportAsync(Guid reportId)
        {
            ValidateGuid(reportId, nameof(reportId));

            var reports = await LoadReportsAsync();
            var removed = reports.RemoveAll(r => r.Id == reportId) > 0;

            if (removed)
                await SaveReportsAsync(reports);

            return removed;
        }

        // ========================= Helpers =========================
        // 📈 يحسب اتجاه أداء الطالب بمقارنة متوسط درجاته في آخر 30 يوم مع الفترة السابقة (30-60 يوم)، ويرجع:
        // "Improving" إذا تحسّن، "Declining" إذا تدهور، أو "Stable" إذا لم يتغير بشكل ملحوظ
        private static string CalculateStudentTrend(List<ProgressEntry> entries)
        {
            if (entries.Count < 2) return "Stable";

            var now = DateTime.UtcNow;

            var lastAvg = entries
                .Where(p => p.date >= now.AddDays(-TrendLastDays))
                .Select(p => (double)p.level_score)
                .DefaultIfEmpty(0)
                .Average();

            var prevAvg = entries
                .Where(p => p.date >= now.AddDays(-TrendPreviousDays) && p.date < now.AddDays(-TrendLastDays))
                .Select(p => (double)p.level_score)
                .DefaultIfEmpty(0)
                .Average();

            if (lastAvg > prevAvg + TrendThreshold) return "Improving";
            if (lastAvg < prevAvg - TrendThreshold) return "Declining";
            return "Stable";
        }
        // 🔐 دالة مساعدة للتحقق من صحة المعرفات: ترمي استثناء إذا كانت القيمة فارغة (Guid.Empty) لمنع الأخطاء اللاحقة
        private static void ValidateGuid(Guid value, string paramName)
        {
            if (value == Guid.Empty)
                throw new ArgumentException("Invalid Guid value.", paramName);
        }
        // ⚙️ إعدادات موحدة لـ JSON: تجاهل حالة أحرف الخصائص + تنسيق الإخراج بقراءته (للتصحيح)
        private static JsonSerializerOptions JsonOptions => new()
        {
            PropertyNameCaseInsensitive = true,
            WriteIndented = true
        };
        // 📂 يحمّل قائمة التقارير من ملف JSON بشكل غير متزامن وآمن: يتحقق من وجود الملف ومحتواه، يحول الـ JSON إلى كائنات، ويضمن إفلات القفل دائماً عبر finally
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
        // 💾 يحفظ قائمة التقارير في ملف JSON بشكل غير متزامن وآمن: يحوّل البيانات لنص منسّق، يكتبها على القرص، ويضمن إفلات قفل التزامن دائماً عبر finally
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
    }
}
