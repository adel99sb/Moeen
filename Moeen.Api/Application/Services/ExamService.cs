using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Exam;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Exam;

namespace Moeen.Api.Application.Services
{
    public class ExamService : IExamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public ExamService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Exam CRUD Operations

        public async Task<GeneralResponse> CreateExamAsync(CreateExamRequest createRequest)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(createRequest.StudentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var teacher = await _userManager.FindByIdAsync(createRequest.TeacherId.ToString());
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher/Examiner not found.");

                var exam = new Exam
                {
                    Id = Guid.NewGuid(),
                    StudentId = createRequest.StudentId,
                    TeacherId = createRequest.TeacherId,
                    JuzForm = createRequest.JuzForm,
                    JuzTo = createRequest.JuzTo,
                    Mark = createRequest.Mark,
                    Date = createRequest.Date ?? DateTime.Now
                };

                await _unitOfWork.Repository<Exam>().AddAsync(exam);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Exam record created successfully.", new { exam.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to create exam record: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateExamAsync(Guid examId, UpdateExamRequest updateRequest)
        {
            try
            {
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
                if (exam == null)
                    return GeneralResponse.NotFound("Exam record not found.");

                exam.JuzForm = updateRequest.JuzForm;
                exam.JuzTo = updateRequest.JuzTo;
                exam.Mark = updateRequest.Mark;

                await _unitOfWork.Repository<Exam>().UpdateAsync(exam);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Exam record updated successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update exam record: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteExamAsync(Guid examId)
        {
            try
            {
                var exam = await _unitOfWork.Repository<Exam>().GetByIdAsync(examId);
                if (exam == null)
                    return GeneralResponse.NotFound("Exam record not found.");

                await _unitOfWork.Repository<Exam>().DeleteAsync(exam);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Exam record deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete exam record: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetExamByIdAsync(Guid examId)
        {
            try
            {
                var spec = Spec.For<Exam>(e => e.Id == examId);
                spec.AddInclude(e => e.Student);
                spec.AddInclude(e => e.Teacher);

                var exam = (await _unitOfWork.Repository<Exam>().GetAllAsync(spec)).FirstOrDefault();
                if (exam == null)
                    return GeneralResponse.NotFound("Exam record not found.");

                var response = new ExamResponse
                {
                    Id = exam.Id,
                    StudentId = exam.StudentId,
                    StudentFullName = exam.Student?.FullName,
                    TeacherId = exam.TeacherId,
                    TeacherFullName = exam.Teacher?.FullName,
                    JuzForm = exam.JuzForm,
                    JuzTo = exam.JuzTo,
                    Mark = exam.Mark,
                    Date = exam.Date
                };

                return GeneralResponse.Ok("Exam record retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve exam record: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        public async Task<GeneralResponse> GetStudentExamsAsync(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var spec = Spec.For<Exam>(e => e.StudentId == studentId);
                spec.AddInclude(e => e.Student);
                spec.AddInclude(e => e.Teacher);

                var exams = await _unitOfWork.Repository<Exam>().GetAllAsync(spec);

                var response = exams.Select(e => new ExamResponse
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentFullName = e.Student?.FullName,
                    TeacherId = e.TeacherId,
                    TeacherFullName = e.Teacher?.FullName,
                    JuzForm = e.JuzForm,
                    JuzTo = e.JuzTo,
                    Mark = e.Mark,
                    Date = e.Date
                }).OrderByDescending(e => e.Date).ToList();

                return GeneralResponse.Ok($"Exams for student '{student.FullName}' retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve student exams: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetExamsByTeacherAndDateAsync(Guid teacherId, DateTime? date)
        {
            try
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher not found.");

                DateTime targetDate = date.HasValue ? date.Value.Date : DateTime.Today;
                DateTime startOfDay = targetDate;
                DateTime endOfDay = targetDate.AddDays(1).AddTicks(-1);

                var spec = Spec.For<Exam>(e =>
                    e.TeacherId == teacherId &&
                    e.Date >= startOfDay &&
                    e.Date <= endOfDay);

                spec.AddInclude(e => e.Student);
                spec.AddInclude(e => e.Teacher);

                var exams = await _unitOfWork.Repository<Exam>().GetAllAsync(spec);

                var response = exams.Select(e => new ExamResponse
                {
                    Id = e.Id,
                    StudentId = e.StudentId,
                    StudentFullName = e.Student?.FullName,
                    TeacherId = e.TeacherId,
                    TeacherFullName = e.Teacher?.FullName,
                    JuzForm = e.JuzForm,
                    JuzTo = e.JuzTo,
                    Mark = e.Mark,
                    Date = e.Date
                }).ToList();

                return GeneralResponse.Ok($"Exams conducted by teacher '{teacher.FullName}' on {targetDate.ToShortDateString()} retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve teacher exams: {ex.Message}");
            }
        }

        #endregion
    }
}