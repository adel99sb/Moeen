using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Constants;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Dashboard;

namespace Moeen.Api.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;

        public DashboardService(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task<GeneralResponse> GetAdminDashboardAsync()
        {
            try
            {
                var totalMosques = (await _unitOfWork.Repository<Mosque>().GetAllAsync()).Count();

                var allUsers = _userManager.Users.ToList();
                var totalTeachers = allUsers.Count(u => u.UserRole == UserRole.Teacher);
                var totalStudents = allUsers.Count(u => u.UserRole == UserRole.Student);

                var complaintSpec = Spec.For<Complaint>(c => c.Status == ComplaintStatus.Pending);
                var pendingComplaints = (await _unitOfWork.Repository<Complaint>().GetAllAsync(complaintSpec)).Count();

                var data = new AdminDashboardDto
                {
                    TotalMosques = totalMosques,
                    TotalTeachers = totalTeachers,
                    TotalStudents = totalStudents,
                    PendingComplaintsCount = pendingComplaints
                };

                return GeneralResponse.Ok("Admin dashboard data retrieved successfully.", data);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Dashboard error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetMosqueDashboardAsync(Guid mosqueId)
        {
            try
            {
                var mosque = await _unitOfWork.Repository<Mosque>().GetByIdAsync(mosqueId);
                if (mosque == null)
                    return GeneralResponse.NotFound("Mosque not found.");

                var userSpec = Spec.For<MosqueUser>(mu => mu.MosqueId == mosqueId);
                var totalStudents = (await _unitOfWork.Repository<MosqueUser>().GetAllAsync(userSpec)).Count();

                DateTime startOfDay = DateTime.Today;
                DateTime endOfDay = DateTime.Today.AddDays(1).AddTicks(-1);

                var attendanceSpec = Spec.For<Attendance>(a => a.MosqueId == mosqueId && a.AttendanceDate >= startOfDay && a.AttendanceDate <= endOfDay);
                var todayAttendance = await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec);

                var postSpec = Spec.For<Post>(p => p.MosqueId == mosqueId);
                var totalPosts = (await _unitOfWork.Repository<Post>().GetAllAsync(postSpec)).Count();

                var data = new MosqueDashboardDto
                {
                    MosqueId = mosqueId,
                    MosqueName = mosque.Name,
                    TotalStudentsInMosque = totalStudents,
                    TodayPresentStudents = todayAttendance.Count(a => a.Status == AttendanceStatus.Present),
                    TodayAbsentStudents = todayAttendance.Count(a => a.Status == AttendanceStatus.Absent),
                    TotalPostsCount = totalPosts
                };

                return GeneralResponse.Ok("Mosque dashboard data retrieved successfully.", data);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Dashboard error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetTeacherDashboardAsync(Guid teacherId)
        {
            try
            {
                var teacher = await _userManager.FindByIdAsync(teacherId.ToString());
                if (teacher == null)
                    return GeneralResponse.NotFound("Teacher not found.");

                var progressSpec = Spec.For<ProgressEntry>(p => p.TeacherId == teacherId);
                var teacherEntries = await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec);
                var distinctStudentsCount = teacherEntries.Select(p => p.StudentId).Distinct().Count();

                var todayEntriesCount = teacherEntries.Count();

                var examSpec = Spec.For<Exam>(e => e.TeacherId == teacherId);
                var totalExams = (await _unitOfWork.Repository<Exam>().GetAllAsync(examSpec)).Count();

                var data = new TeacherDashboardDto
                {
                    TotalStudentsUnderSupervision = distinctStudentsCount,
                    TotalProgressEntriesCount = todayEntriesCount,
                    TotalExamsConducted = totalExams
                };

                return GeneralResponse.Ok("Teacher dashboard data retrieved successfully.", data);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Dashboard error: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetStudentDashboardAsync(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var attendanceSpec = Spec.For<Attendance>(a => a.StudentId == studentId);
                var attendanceRecords = await _unitOfWork.Repository<Attendance>().GetAllAsync(attendanceSpec);
                double attendanceRate = 0;
                if (attendanceRecords.Count() > 0)
                {
                    var presentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
                    attendanceRate = Math.Round(((double)presentCount / attendanceRecords.Count()) * 100, 2);
                }

                var examSpec = Spec.For<Exam>(e => e.StudentId == studentId);
                var exams = await _unitOfWork.Repository<Exam>().GetAllAsync(examSpec);
                double avgMark = exams.Count() > 0 ? Math.Round(exams.Average(e => e.Mark), 2) : 0;

                var progressSpec = Spec.For<ProgressEntry>(p => p.StudentId == studentId);
                var progressRecords = await _unitOfWork.Repository<ProgressEntry>().GetAllAsync(progressSpec);
                var lastEntry = progressRecords.OrderByDescending(p => p.Date).FirstOrDefault();

                var data = new StudentDashboardDto
                {
                    AttendanceRate = attendanceRate,
                    TotalExamsPassed = exams.Count(),
                    AverageExamMark = avgMark,
                    LastMemorizedJuz = lastEntry?.JuzNumber ?? 0,
                    LastMemorizedPage = lastEntry?.PageNumber ?? 0
                };

                return GeneralResponse.Ok("Student dashboard data retrieved successfully.", data);
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Dashboard error: {ex.Message}");
            }
        }
    }
}