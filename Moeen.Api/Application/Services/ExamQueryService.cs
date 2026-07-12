using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ExamCommand;
using Moeen.Shared.Responses.ExamQuery;
using Moeen.Shared.Responses.ExamQuery.Moeen.Shared.Responses.ExamQuery;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class ExamQueryService : IExamQueryService
    {
        private readonly AppDbContext _context;

        public ExamQueryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request)
        {
            if (request == null || request.HalqaId == null || !request.HalqaId.Any())
                return GeneralResponse.BadRequest("معرفات الحلقات مطلوبة.");

            var fromDate = request.FromDate ?? DateTime.MinValue;
            var toDate = request.ToDate ?? DateTime.MaxValue;

            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                    .ThenInclude(s => s.Halqa)
                .Where(e => e.Student != null && e.Student.HalqaId.HasValue && request.HalqaId.Contains(e.Student.HalqaId.Value) && e.date >= fromDate && e.date <= toDate);

            var performance = await query
                .GroupBy(e => e.Student.Halqa.Name)
                .Select(g => new HalqaPerformanceDto
                {
                    HalqaName = g.Key,
                    AverageScore = g.Average(e => e.score),
                    ExamsCount = g.Count(),
                    StudentsCount = g.Select(e => e.StudentId).Distinct().Count()
                })
                .OrderByDescending(p => p.AverageScore)
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب مقارنة أداء الحلقات.", performance);
        }

        public async Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var exam = await _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Teacher)
                .FirstOrDefaultAsync(e => e.Id == request.ExamId);

            if (exam == null)
                return GeneralResponse.NotFound("الاختبار غير موجود.");

            var dto = new ExamResultDto
            {
                Id = exam.Id,
                StudentId = exam.StudentId,
                StudentName = exam.Student?.name ?? string.Empty,
                TeacherId = exam.TeacherId,
                TeacherName = exam.Teacher?.name ?? string.Empty,
                HalqaTeacherName = exam.Student?.Halqa?.Teacher?.name ?? string.Empty,
                ExaminerName = exam.Teacher?.name ?? string.Empty,
                JuzFrom = exam.juz_form,
                JuzTo = exam.juz_to,
                Score = exam.score,
                Mark = exam.mark,
                Date = exam.date,
                Notes = exam.notes,
                Grade = CalculateGrade(exam.score)
            };

            return GeneralResponse.Ok("تم جلب نتيجة الاختبار.", dto);
        }

        public async Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Where(e => e.Student != null && e.Student.HalqaId == request.HalqaId);

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            var total = await query.CountAsync();
            var exams = await query
                .OrderByDescending(e => e.date)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ExamSummaryDto
                {
                    ExamId = e.Id,
                    StudentName = e.Student.name,
                    Score = e.score,
                    Date = e.date
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب اختبارات الحلقة.", exams, request.PageNumber, request.PageSize, total);
        }

        //public async Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request)
        //{
        //    // Not implemented due to unclear schema
        //    return GeneralResponse.NotImplemented("GetExamsByPhaseAsync is not implemented.");
        //}

        public async Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Where(e => e.TeacherId == request.TeacherId);

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            var exams = await query
                .OrderByDescending(e => e.date)
                .Select(e => new ExamSummaryDto
                {
                    ExamId = e.Id,
                    StudentName = e.Student.name,
                    Score = e.score,
                    Date = e.date
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب اختبارات المعلم.", exams);
        }

        public async Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request)
        {
            var query = _context.Exams.AsNoTracking();

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            if (!await query.AnyAsync())
                return GeneralResponse.Ok("لا توجد بيانات لعرض الإحصائيات.", new ExamStatisticsDto());

            var stats = new ExamStatisticsDto
            {
                TotalExams = await query.CountAsync(),
                AverageScore = await query.AverageAsync(e => e.score),
                HighestScore = await query.MaxAsync(e => e.score),
                LowestScore = await query.MinAsync(e => e.score),
                PassCount = await query.CountAsync(e => e.score >= 50),
                FailedCount = await query.CountAsync(e => e.score < 50),
                PassRate = (double)await query.CountAsync(e => e.score >= 50) / await query.CountAsync() * 100
            };

            return GeneralResponse.Ok("تم جلب إحصائيات الاختبارات.", stats);
        }

        public async Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Where(e => e.Student != null && e.Student.HalqaId == request.HalqaId);

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            if (!await query.AnyAsync())
                return GeneralResponse.Ok("لا توجد بيانات لعرض التحليلات.", new HalqaExamAnalyticsDto());

            var analytics = new HalqaExamAnalyticsDto
            {
                HalqaId = request.HalqaId,
                AverageScore = await query.AverageAsync(e => e.score),
                ExamsCount = await query.CountAsync(),
                TopStudent = await query.OrderByDescending(e => e.score).Select(e => e.Student.name).FirstOrDefaultAsync(),
                LowestStudent = await query.OrderBy(e => e.score).Select(e => e.Student.name).FirstOrDefaultAsync()
            };

            return GeneralResponse.Ok("تم جلب تحليلات الحلقة.", analytics);
        }

        public async Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e => e.StudentId == request.StudentId)
                .OrderByDescending(e => e.date)
                .Select(e => new ExamResultDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.name : string.Empty,
                    TeacherId = e.TeacherId,
                    TeacherName = e.Teacher != null ? e.Teacher.name : string.Empty,
                    HalqaTeacherName = e.Student != null && e.Student.Halqa != null && e.Student.Halqa.Teacher != null ? e.Student.Halqa.Teacher.name : string.Empty,
                    ExaminerName = e.Teacher != null ? e.Teacher.name : string.Empty,
                    JuzFrom = e.juz_form,
                    JuzTo = e.juz_to,
                    Score = e.score,
                    Mark = e.mark,
                    Date = e.date,
                    Notes = e.notes,
                    Grade = CalculateGrade(e.score)
                })
                .ToListAsync();

            var response = new GetStudentExamsResponse
            {
                Exams = exams,
                TotalCount = exams.Count
            };

            return GeneralResponse.Ok("تم جلب اختبارات الطالب.", response);
        }

        public async Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var exams = await _context.Exams
                .AsNoTracking()
                .Where(e => e.StudentId == request.StudentId && e.date >= request.FromDate && e.date <= request.ToDate)
                .OrderByDescending(e => e.date)
                .Select(e => new ExamResultDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student != null ? e.Student.name : string.Empty,
                    TeacherId = e.TeacherId,
                    TeacherName = e.Teacher != null ? e.Teacher.name : string.Empty,
                    HalqaTeacherName = e.Student != null && e.Student.Halqa != null && e.Student.Halqa.Teacher != null ? e.Student.Halqa.Teacher.name : string.Empty,
                    ExaminerName = e.Teacher != null ? e.Teacher.name : string.Empty,
                    JuzFrom = e.juz_form,
                    JuzTo = e.juz_to,
                    Score = e.score,
                    Mark = e.mark,
                    Date = e.date,
                    Notes = e.notes,
                    Grade = CalculateGrade(e.score)
                })
                .ToListAsync();

            var response = new GetStudentExamsResponse
            {
                Exams = exams,
                TotalCount = exams.Count
            };

            return GeneralResponse.Ok("تم جلب اختبارات الطالب ضمن الفترة المحددة.", response);
        }

        public async Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request)
        {
            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Teacher);

            if (request.FromDate.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Core.Entities.Exam, Core.Entities.Teacher>)query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Core.Entities.Exam, Core.Entities.Teacher>)query.Where(e => e.date <= request.ToDate.Value);

            var data = await query
                .OrderBy(e => e.date)
                .ToListAsync();

            var sb = new StringBuilder();
            sb.AppendLine("ExamId,StudentName,TeacherName,Date,Score,Mark,Notes");
            foreach (var exam in data)
            {
                sb.AppendLine($"{exam.Id},{exam.Student?.name},{exam.Teacher?.name},{exam.date:yyyy-MM-dd},{exam.score},{exam.mark},\"{exam.notes?.Replace("\"", "\"\"")}\"");
            }

            var response = new ExportDataResponse
            {
                FileName = $"Exams_{DateTime.Now:yyyyMMdd}.csv",
                ContentType = "text/csv",
                Content = Encoding.UTF8.GetBytes(sb.ToString())
            };

            return GeneralResponse.Ok("تم تجهيز بيانات التصدير.", response);
        }

        public async Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("الطلب غير صالح.");

            var query = _context.Exams
                .AsNoTracking()
                .Include(e => e.Student)
                .Include(e => e.Teacher)
                .AsQueryable();

            if (request.StudentId.HasValue)
                query = query.Where(e => e.StudentId == request.StudentId.Value);
            if (request.TeacherId.HasValue)
                query = query.Where(e => e.TeacherId == request.TeacherId.Value);
            if (request.MinScore.HasValue)
                query = query.Where(e => e.score >= request.MinScore.Value);
            if (request.MaxScore.HasValue)
                query = query.Where(e => e.score <= request.MaxScore.Value);
            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            var total = await query.CountAsync();
            var results = await query
                .OrderByDescending(e => e.date)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(e => new ExamResultDto
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentName = e.Student.name,
                    TeacherId = e.TeacherId,
                    TeacherName = e.Teacher.name,
                    HalqaTeacherName = e.Student != null && e.Student.Halqa != null && e.Student.Halqa.Teacher != null ? e.Student.Halqa.Teacher.name : string.Empty,
                    ExaminerName = e.Teacher != null ? e.Teacher.name : string.Empty,
                    JuzFrom = e.juz_form,
                    JuzTo = e.juz_to,
                    Score = e.score,
                    Mark = e.mark,
                    Date = e.date,
                    Notes = e.notes,
                    Grade = CalculateGrade(e.score)
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب نتائج البحث.", results, request.PageNumber, request.PageSize, total);
        }

        private static string CalculateGrade(int score)
        {
            if (score >= 90) return "ممتاز";
            if (score >= 80) return "جيد جداً";
            if (score >= 70) return "جيد";
            if (score >= 60) return "مقبول";
            return "يحتاج تحسين";
        }

        public async Task<GeneralResponse> GetTopPerformingStudentsInExamsAsync(GetTopPerformingStudentsInExamsRequest request)
        {
            var query = _context.Exams.AsNoTracking();

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            var topStudents = await query
                .GroupBy(e => e.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    AverageGrade = g.Average(e => e.score),
                    TotalPoints = g.Sum(e => e.mark),
                    ExamsTaken = g.Count()
                })
                .OrderByDescending(s => s.AverageGrade)
                .ThenByDescending(s => s.TotalPoints)
                .Take(request.TopCount)
                .Join(_context.Students,
                      examStat => examStat.StudentId,
                      student => student.Id,
                      (examStat, student) => new ExamStudentPerformanceDto
                      {
                          StudentId = student.Id,
                          StudentName = student.name,
                          ExamsTaken = examStat.ExamsTaken,
                          AverageGrade = Math.Round(examStat.AverageGrade, 2),
                          TotalPoints = examStat.TotalPoints
                      })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الطلاب الأكثر تميزاً.", topStudents);
        }

        public async Task<GeneralResponse> GetLowestPerformingStudentsInExamsAsync(GetLowestPerformingStudentsInExamsRequest request)
        {
            var query = _context.Exams.AsNoTracking();

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            var lowestStudents = await query
                .GroupBy(e => e.StudentId)
                .Select(g => new
                {
                    StudentId = g.Key,
                    AverageGrade = g.Average(e => e.score),
                    TotalPoints = g.Sum(e => e.mark),
                    ExamsTaken = g.Count()
                })
                .OrderBy(s => s.AverageGrade)
                .ThenBy(s => s.TotalPoints)
                .Take(request.BottomCount)
                .Join(_context.Students,
                      examStat => examStat.StudentId,
                      student => student.Id,
                      (examStat, student) => new ExamStudentPerformanceDto
                      {
                          StudentId = student.Id,
                          StudentName = student.name,
                          ExamsTaken = examStat.ExamsTaken,
                          AverageGrade = Math.Round(examStat.AverageGrade, 2),
                          TotalPoints = examStat.TotalPoints
                      })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب الطلاب الأقل تميزاً.", lowestStudents);
        }

        public async Task<GeneralResponse> GetLabStatisticsAsync(GetLabStatisticsRequest request)
        {
            var query = _context.Exams.AsNoTracking();

            if (request.FromDate.HasValue)
                query = query.Where(e => e.date >= request.FromDate.Value);
            if (request.ToDate.HasValue)
                query = query.Where(e => e.date <= request.ToDate.Value);

            if (!await query.AnyAsync())
                return GeneralResponse.Ok("لا توجد بيانات لعرض الإحصائيات.", new LabStatisticsDto());

            var totalExams = await query.CountAsync();
            var totalStudents = await query.Select(e => e.StudentId).Distinct().CountAsync();
            var averageGrade = await query.AverageAsync(e => e.score);
            var passCount = await query.CountAsync(e => e.score >= 50);
            var totalPoints = await query.SumAsync(e => e.mark);

            var studentPerformance = query
                .GroupBy(e => e.StudentId)
                .Select(g => new { StudentId = g.Key, AverageGrade = g.Average(e => e.score) });

            var topStudentId = await studentPerformance.OrderByDescending(p => p.AverageGrade).Select(p => p.StudentId).FirstOrDefaultAsync();
            var lowestStudentId = await studentPerformance.OrderBy(p => p.AverageGrade).Select(p => p.StudentId).FirstOrDefaultAsync();

            var topStudentName = await _context.Students.Where(s => s.Id == topStudentId).Select(s => s.name).FirstOrDefaultAsync();
            var lowestStudentName = await _context.Students.Where(s => s.Id == lowestStudentId).Select(s => s.name).FirstOrDefaultAsync();

            var stats = new LabStatisticsDto
            {
                TotalExamsConducted = totalExams,
                TotalStudentsTested = totalStudents,
                AverageGrade = Math.Round(averageGrade, 2),
                PassRate = totalExams > 0 ? Math.Round((double)passCount / totalExams * 100, 2) : 0,
                TotalPointsAwarded = totalPoints,
                TopPerformingStudent = topStudentName,
                LowestPerformingStudent = lowestStudentName
            };

            return GeneralResponse.Ok("تم جلب إحصائيات المختبر.", stats);
        }
    }
}