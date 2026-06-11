using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Progress;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Progress;

namespace Moeen.Api.Application.Services
{
    public class ProgressService : IProgressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public ProgressService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Progress Entry CRUD Operations

        public async Task<GeneralResponse> CreateProgressEntryAsync(CreateProgressEntryRequest createRequest)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(createRequest.StudentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var teacher = await _userManager.FindByIdAsync(createRequest.TeacherId.ToString());
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher not found.");

                var progressEntry = new ProgressEntry
                {
                    Id = Guid.NewGuid(),
                    StudentId = createRequest.StudentId,
                    TeacherId = createRequest.TeacherId,
                    JuzNumber = createRequest.JuzNumber,
                    PageNumber = createRequest.PageNumber,
                    Date = createRequest.Date ?? DateTime.Now
                };

                await _unitOfWork.Repository<ProgressEntry>().AddAsync(progressEntry);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Progress entry recorded successfully.", new { progressEntry.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to record progress entry: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateProgressEntryAsync(Guid entryId, UpdateProgressEntryRequest updateRequest)
        {
            try
            {
                var entry = await _unitOfWork.Repository<ProgressEntry>().GetByIdAsync(entryId);
                if (entry == null)
                    return GeneralResponse.NotFound("Progress entry not found.");

                entry.JuzNumber = updateRequest.JuzNumber;
                entry.PageNumber = updateRequest.PageNumber;

                await _unitOfWork.Repository<ProgressEntry>().UpdateAsync(entry);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Progress entry updated successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update progress entry: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteProgressEntryAsync(Guid entryId)
        {
            try
            {
                var entry = await _unitOfWork.Repository<ProgressEntry>().GetByIdAsync(entryId);
                if (entry == null)
                    return GeneralResponse.NotFound("Progress entry not found.");

                await _unitOfWork.Repository<ProgressEntry>().DeleteAsync(entry);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Progress entry deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete progress entry: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetProgressEntryByIdAsync(Guid entryId)
        {
            try
            {
                var spec = Spec.For<ProgressEntry>(p => p.Id == entryId);
                spec.AddInclude(p => p.Student);
                spec.AddInclude(p => p.Teacher);

                var entry = (await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(spec)).FirstOrDefault();
                if (entry == null)
                    return GeneralResponse.NotFound("Progress entry not found.");

                var response = new ProgressEntryResponse
                {
                    Id = entry.Id,
                    StudentId = entry.StudentId,
                    StudentFullName = entry.Student?.FullName,
                    TeacherId = entry.TeacherId,
                    TeacherFullName = entry.Teacher?.FullName,
                    JuzNumber = entry.JuzNumber,
                    PageNumber = entry.PageNumber,
                    Date = entry.Date
                };

                return GeneralResponse.Ok("Progress entry retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve progress entry: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        public async Task<GeneralResponse> GetStudentProgressHistoryAsync(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var spec = Spec.For<ProgressEntry>(p => p.StudentId == studentId);
                spec.AddInclude(p => p.Student);
                spec.AddInclude(p => p.Teacher);

                var entries = await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(spec);

                var response = entries.Select(p => new ProgressEntryResponse
                {
                    Id = p.Id,
                    StudentId = p.StudentId,
                    StudentFullName = p.Student?.FullName,
                    TeacherId = p.TeacherId,
                    TeacherFullName = p.Teacher?.FullName,
                    JuzNumber = p.JuzNumber,
                    PageNumber = p.PageNumber,
                    Date = p.Date
                }).OrderByDescending(p => p.Date).ToList();

                return GeneralResponse.Ok($"Progress history for student '{student.FullName}' retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve student progress history: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetTeacherEntriesByDateAsync(Guid teacherId, DateTime? date)
        {
            try
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher not found.");

                DateTime targetDate = date.HasValue ? date.Value.Date : DateTime.Today;
                DateTime startOfDay = targetDate;
                DateTime endOfDay = targetDate.AddDays(1).AddTicks(-1);

                var spec = Spec.For<ProgressEntry>(p =>
                    p.TeacherId == teacherId &&
                    p.Date >= startOfDay &&
                    p.Date <= endOfDay);

                spec.AddInclude(p => p.Student);
                spec.AddInclude(p => p.Teacher);

                var entries = await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(spec);

                var response = entries.Select(p => new ProgressEntryResponse
                {
                    Id = p.Id,
                    StudentId = p.StudentId,
                    StudentFullName = p.Student?.FullName,
                    TeacherId = p.TeacherId,
                    TeacherFullName = p.Teacher?.FullName,
                    JuzNumber = p.JuzNumber,
                    PageNumber = p.PageNumber,
                    Date = p.Date
                }).ToList();

                return GeneralResponse.Ok($"Progress entries recorded by teacher '{teacher.FullName}' on {targetDate.ToShortDateString()} retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve teacher progress entries: {ex.Message}");
            }
        }

        #endregion
    }
}