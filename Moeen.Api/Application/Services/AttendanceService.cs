using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.Attendance;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Attendance;

namespace Moeen.Api.Application.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public AttendanceService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        #region Attendance CRUD Operations

        public async Task<GeneralResponse> RecordAttendanceAsync(CreateAttendanceRequest createAttendanceRequest)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(createAttendanceRequest.StudentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(createAttendanceRequest.MosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var dateOnly = createAttendanceRequest.AttendanceDate.Date;
                var duplicateSpec = Spec.For<Attendance>(a =>
                    a.StudentId == createAttendanceRequest.StudentId &&
                    a.MosqueId == createAttendanceRequest.MosqueId &&
                    a.AttendanceDate.Date == dateOnly);

                var existingRecord = (await _unitOfWork.Repository<Attendance>().GetAllAsync(duplicateSpec)).FirstOrDefault();
                if (existingRecord != null)
                    return GeneralResponse.BadRequest("Attendance already recorded for this student at the specified date.");

                var attendance = new Attendance
                {
                    Id = Guid.NewGuid(),
                    StudentId = createAttendanceRequest.StudentId,
                    MosqueId = createAttendanceRequest.MosqueId,
                    AttendanceDate = createAttendanceRequest.AttendanceDate.Date,
                    Status = createAttendanceRequest.Status,
                    Note = createAttendanceRequest.Note
                };

                await _unitOfWork.Repository<Attendance>().AddAsync(attendance);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Attendance recorded successfully.", new { attendance.Id });
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to record attendance: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UpdateAttendanceAsync(Guid attendanceId, UpdateAttendanceRequest updateAttendanceRequest)
        {
            try
            {
                var attendance = await _unitOfWork.Repository<Attendance>().GetByIdAsync(attendanceId);
                if (attendance == null)
                    return GeneralResponse.NotFound("Attendance record not found.");

                attendance.Status = updateAttendanceRequest.Status;
                attendance.Note = updateAttendanceRequest.Note;

                await _unitOfWork.Repository<Attendance>().UpdateAsync(attendance);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Attendance record updated successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to update attendance record: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> DeleteAttendanceAsync(Guid attendanceId)
        {
            try
            {
                var attendance = await _unitOfWork.Repository<Attendance>().GetByIdAsync(attendanceId);
                if (attendance == null)
                    return GeneralResponse.NotFound("Attendance record not found.");

                await _unitOfWork.Repository<Attendance>().DeleteAsync(attendance);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Attendance record deleted successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to delete attendance record: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetAttendanceByIdAsync(Guid attendanceId)
        {
            try
            {
                var spec = Spec.For<Attendance>(a => a.Id == attendanceId);
                spec.AddInclude(a => a.Student);
                spec.AddInclude(a => a.Mosque);

                var attendance = (await _unitOfWork.Repository<Attendance>().GetAllAsync(spec)).FirstOrDefault();
                if (attendance == null)
                    return GeneralResponse.NotFound("Attendance record not found.");

                var response = new AttendanceResponse
                {
                    Id = attendance.Id,
                    StudentId = attendance.StudentId,
                    StudentFullName = attendance.Student?.FullName,
                    MosqueId = attendance.MosqueId,
                    MosqueName = attendance.Mosque?.Name,
                    AttendanceDate = attendance.AttendanceDate,
                    Status = attendance.Status,
                    Note = attendance.Note
                };

                return GeneralResponse.Ok("Attendance record retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve attendance record: {ex.Message}");
            }
        }

        #endregion

        #region Custom Operations

        public async Task<GeneralResponse> GetStudentAttendanceHistoryAsync(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var spec = Spec.For<Attendance>(a => a.StudentId == studentId);
                spec.AddInclude(a => a.Student);
                spec.AddInclude(a => a.Mosque);

                var history = await _unitOfWork.Repository<Attendance>().GetAllAsync(spec);

                var response = history.Select(a => new AttendanceResponse
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentFullName = a.Student?.FullName,
                    MosqueId = a.MosqueId,
                    MosqueName = a.Mosque?.Name,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status,
                    Note = a.Note
                }).OrderByDescending(a => a.AttendanceDate).ToList();

                return GeneralResponse.Ok($"Attendance history for student {student.FullName} retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve student attendance history: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetMosqueAttendanceByDateAsync(Guid mosqueId, DateTime? date)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                DateTime targetDate = date.HasValue ? date.Value.Date : DateTime.Today;

                DateTime startOfDay = targetDate;
                DateTime endOfDay = targetDate.AddDays(1).AddTicks(-1);

                var spec = Spec.For<Attendance>(a =>
                    a.MosqueId == mosqueId &&
                    a.AttendanceDate >= startOfDay &&
                    a.AttendanceDate <= endOfDay);
                spec.AddInclude(a => a.Student);
                spec.AddInclude(a => a.Mosque);

                var records = await _unitOfWork.Repository<Attendance>().GetAllAsync(spec);

                var response = records.Select(a => new AttendanceResponse
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentFullName = a.Student?.FullName,
                    MosqueId = a.MosqueId,
                    MosqueName = a.Mosque?.Name,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status,
                    Note = a.Note
                }).ToList();

                return GeneralResponse.Ok($"Attendance sheets for mosque '{mosque.Name}' on {targetDate.ToShortDateString()} retrieved successfully.", response);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve mosque attendance sheets: {ex.Message}");
            }
        }

        #endregion
    }
}