using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.ExamQuery;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.ExamCommand;
using Moeen.Shared.Responses.ExamQuery;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class ExamQueryService : IExamQueryService   
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExamQueryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        private ExamResultDto MapExamToDto(Exam e, string studentName = "", string teacherName = "")
        {
            if (e == null) return null;
            return new ExamResultDto
            {
                Id = e.Id,
                StudentId = e.StudentId,
                StudentName = studentName,
                TeacherId = e.TeacherId,
                TeacherName = teacherName,
                JuzFrom = e.juz_form,
                JuzTo = e.juz_to,
                Score = e.score,
                Date = e.date,
                Notes = e.notes,
                Mark = e.mark,
                Grade = e.score.ToString() // fallback; consumer may compute grade differently
            };
        }

        public async Task<GeneralResponse> GetExamResultByIdAsync(GetExamResultByIdRequest request)
        {
            try
            {
                if (request == null || request.ExamId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف الاختبار غير صالح");

                var spec = Spec.ForChain<Exam>(
                    e => e.Id == request.ExamId,
                    q => q.Include(x => x.Student).Include(x => x.Teacher)
                );

                var exams = await _unitOfWork.Repository<Exam>().GetAllAsync(spec);
                var exam = exams.FirstOrDefault();
                if (exam == null)
                    return GeneralResponse.NotFound("نتيجة الاختبار غير موجودة");

                var dto = MapExamToDto(exam, exam.Student?.name ?? string.Empty, exam.Teacher?.name ?? string.Empty);
                return GeneralResponse.Ok("تم جلب نتيجة الاختبار", dto);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب نتيجة الاختبار", ex.Message);
            }
        }

        public async Task<GeneralResponse> SearchExamResultsAsync(SearchExamResultsRequest request)
        {
            try
            {
                if (request == null)
                    request = new SearchExamResultsRequest { Criteria = new ExamResultSearchCriteria() };

                var c = request.Criteria ?? new ExamResultSearchCriteria();
                int page = Math.Max(1, c.PageNumber);
                int pageSize = Math.Max(1, c.PageSize);
                int skip = (page - 1) * pageSize;

                var spec = Spec.ForChain<Exam>(
                    e =>
                        (!c.StudentId.HasValue || e.StudentId == c.StudentId) &&
                        (!c.TeacherId.HasValue || e.TeacherId == c.TeacherId) &&
                        (!c.DateFrom.HasValue || e.date >= c.DateFrom.Value) &&
                        (!c.DateTo.HasValue || e.date <= c.DateTo.Value) &&
                        (!c.MinScore.HasValue || e.score >= c.MinScore.Value) &&
                        (!c.MaxScore.HasValue || e.score <= c.MaxScore.Value) &&
                        (!c.JuzFrom.HasValue || e.juz_form >= c.JuzFrom.Value) &&
                        (!c.JuzTo.HasValue || e.juz_to <= c.JuzTo.Value),
                    q => q.Include(x => x.Student).Include(x => x.Teacher)
                );

                spec.ApplyOrderByDescending(e => e.date);
                spec.ApplyPaging(skip, pageSize);

                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var results = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();

                // total count (without paging)
                var countSpec = Spec.For<Exam>(spec.Predicate); // reuse predicate if Spec exposes it; if not, fallback
                var total = (await _unitOfWork.Repository<Exam>().GetAllAsync(countSpec)).Count();

                var response = new SearchExamResultsResponse
                {
                    Results = results,
                    TotalCount = total,
                    PageNumber = page,
                    PageSize = pageSize
                };

                return GeneralResponse.Ok("تم البحث في نتائج الاختبارات", response, page, pageSize, total);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء البحث في نتائج الاختبارات", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetStudentExamsAsync(GetStudentExamsRequest request)
        {
            try
            {
                if (request == null || request.StudentId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف الطالب غير صالح");

                var spec = Spec.ForChain<Exam>(e => e.StudentId == request.StudentId, q => q.Include(x => x.Teacher));
                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();

                var dtos = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();

                var resp = new GetStudentExamsResponse { Exams = dtos, TotalCount = dtos.Count };
                return GeneralResponse.Ok("تم جلب اختبارات الطالب", resp);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب اختبارات الطالب", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetExamsByHalqaAsync(GetExamsByHalqaRequest request)
        {
            try
            {
                if (request == null || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف الحلقة غير صالح");

                int page = Math.Max(1, request.PageNumber);
                int pageSize = Math.Max(1, request.PageSize);
                int skip = (page - 1) * pageSize;

                var spec = Spec.ForChain<Exam>(
                    e => e.Student != null && e.Student.HalqaId == request.HalqaId &&
                         (!request.FromDate.HasValue || e.date >= request.FromDate.Value) &&
                         (!request.ToDate.HasValue || e.date <= request.ToDate.Value),
                    q => q.Include(x => x.Student).Include(x => x.Teacher)
                );

                spec.ApplyOrderByDescending(e => e.date);
                spec.ApplyPaging(skip, pageSize);

                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var dtos = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();

                var totalSpec = Spec.For<Exam>(spec.Predicate);
                var total = (await _unitOfWork.Repository<Exam>().GetAllAsync(totalSpec)).Count();

                var paged = new PagedList<ExamResultDto>
                {
                    Items = dtos,
                    PageNumber = page,
                    PageSize = pageSize,
                    TotalCount = total
                };

                return GeneralResponse.Ok("تم جلب اختبارات الحلقة", paged, page, pageSize, total);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب اختبارات الحلقة", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetStudentExamsByDateRangeAsync(GetStudentExamsByDateRequest request)
        {
            try
            {
                if (request == null || request.StudentId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف الطالب غير صالح");

                var spec = Spec.ForChain<Exam>(
                    e => e.StudentId == request.StudentId && e.date >= request.FromDate && e.date <= request.ToDate,
                    q => q.Include(x => x.Teacher)
                );

                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var dtos = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();
                return GeneralResponse.Ok("تم جلب اختبارات الطالب ضمن الفترة", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب اختبارات الطالب ضمن الفترة", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetExamsByTeacherAsync(GetExamsByTeacherRequest request)
        {
            try
            {
                if (request == null || request.TeacherId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف المعلم غير صالح");

                var spec = Spec.ForChain<Exam>(
                    e => e.TeacherId == request.TeacherId &&
                         (!request.FromDate.HasValue || e.date >= request.FromDate.Value) &&
                         (!request.ToDate.HasValue || e.date <= request.ToDate.Value),
                    q => q.Include(x => x.Student)
                );

                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var dtos = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();
                return GeneralResponse.Ok("تم جلب اختبارات المعلم", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب اختبارات المعلم", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetExamsByPhaseAsync(GetExamsByPhaseRequest request)
        {
            try
            {
                if (request == null || request.PhaseId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف المرحلة غير صالح");

                // تبسيط: نبحث عن الاختبارات التي تحمل PhaseId في notes أو علامته الخاصة إن كان موجوداً
                var spec = Spec.ForChain<Exam>(e => e.notes != null && e.notes.Contains(request.PhaseId.ToString()), q => q.Include(x => x.Student));
                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var dtos = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();
                return GeneralResponse.Ok("تم جلب اختبارات المرحلة", dtos);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب اختبارات المرحلة", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetExamStatisticsAsync(GetExamStatisticsRequest request)
        {
            try
            {
                var repo = _unitOfWork.Repository<Exam>();
                var spec = Spec.For<Exam>(e => true);
                var exams = (await repo.GetAllAsync(spec)).ToList();

                var total = exams.Count;
                var avg = total > 0 ? exams.Average(e => e.score) : 0.0;
                var passed = exams.Count(e => e.score >= 60);
                var failed = total - passed;
                var successRate = total > 0 ? (passed / (double)total) * 100.0 : 0.0;

                var dto = new ExamStatisticsDto
                {
                    TotalExams = total,
                    AverageScore = Math.Round(avg, 2),
                    SuccessRate = Math.Round(successRate, 2),
                    PassedCount = passed,
                    FailedCount = failed
                };

                return GeneralResponse.Ok("تم حساب إحصائيات الاختبارات", dto);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء حساب إحصائيات الاختبارات", ex.Message);
            }
        }

        public async Task<GeneralResponse> GetHalqeExamAnalyticsAsync(GetHalqaAnalyticsRequest request)
        {
            try
            {
                if (request == null || request.HalqaId == Guid.Empty)
                    return GeneralResponse.BadRequest("معرّف الحلقة غير صالح");

                // تعديل الـ Specification ليشمل Include و ThenInclude
                var spec = Spec.ForChain<Exam>(
                    e => e.Student != null && e.Student.HalqaId == request.HalqaId,
                    q => q.Include(x => x.Student)
                          .ThenInclude(s => s.Halqa)   // إضافة ThenInclude لتحميل Halqa
                          .Include(x => x.Teacher)
                );

                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                if (!exams.Any())
                    return GeneralResponse.Ok("لا توجد بيانات للاختبارات في هذه الحلقة", new HalqaExamAnalyticsDto { HalqaId = request.HalqaId, HalqaName = string.Empty });

                var avg = exams.Average(e => e.score);
                var passed = exams.Count(e => e.score >= 60);
                var total = exams.Count;
                var successRate = total > 0 ? (passed / (double)total) * 100.0 : 0.0;

                var dto = new HalqaExamAnalyticsDto
                {
                    HalqaId = request.HalqaId,
                    HalqaName = exams.FirstOrDefault()?.Student?.Halqa?.Name ?? string.Empty,   // تعديل: Halqa?.Name بدلاً من HalqaId?.Name
                    AverageScore = Math.Round(avg, 2),
                    SuccessRate = Math.Round(successRate, 2),
                    TopStudents = exams.OrderByDescending(e => e.score).Take(5).Select(e => new StudentExamPerformanceDto 
                    { StudentId = e.StudentId, StudentName = e.Student?.name ?? string.Empty, AverageScore = e.score }).ToList(),
                    LowStudents = exams.OrderBy(e => e.score).Take(5).Select(e => new StudentExamPerformanceDto
                    { StudentId = e.StudentId, StudentName = e.Student?.name ?? string.Empty, AverageScore = e.score }).ToList()
                };

                return GeneralResponse.Ok("تم جلب تحليلات الاختبارات للحلقة", dto);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء جلب تحليلات الاختبارات للحلقة", ex.Message);
            }
        }

        public async Task<GeneralResponse> CompareHalqasPerformanceAsync(CompareHalqasRequest request)
        {
            try
            {
                // تبسيط: تنفيذ مقارنة حسب متوسط الدرجات لكل حلقة في القائمة
                if (request == null || request.HalqaId == null || !request.HalqaId.Any())
                    return GeneralResponse.BadRequest("قائمة الحلقات مطلوبة للمقارنة");

                var items = new HalqaComparisonDto();

                foreach (var hid in request.HalqaId)
                {
                    var spec = Spec.ForChain<Exam>(e => e.Student != null && e.Student.HalqaId == hid, q => q.Include(x => x.Student));
                    var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                    var avg = exams.Any() ? exams.Average(e => e.score) : 0.0;
                    items.Items.Add(new HalqaComparisonItemDto { HalqaId = hid, AverageScore = Math.Round(avg, 2), TotalExams = exams.Count });
                }

                return GeneralResponse.Ok("تمت المقارنة بين الحلقات", items);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء مقارنة أداء الحلقات", ex.Message);
            }
        }

        public async Task<GeneralResponse> PrepareExamDataForExportAsync(PrepareExportRequest request)
        {
            try
            {
                var spec = Spec.ForChain<Exam>(e => true, q => q.Include(x => x.Student).Include(x => x.Teacher));
                var exams = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).ToList();
                var rows = exams.Select(e => MapExamToDto(e, e.Student?.name ?? string.Empty, e.Teacher?.name ?? string.Empty)).ToList();

                var dto = new ExportExamDataDto { Rows = rows, TotalCount = rows.Count };
                return GeneralResponse.Ok("تم تجهيز بيانات التصدير", dto);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError("حدث خطأ أثناء تجهيز بيانات التصدير", ex.Message);
            }
        }

        public Task<GeneralResponse> GetHalqaExamAnalyticsAsync(GetHalqaAnalyticsRequest request)
        {
            throw new NotImplementedException();
        }
    }
}
