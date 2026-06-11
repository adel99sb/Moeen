using Microsoft.AspNetCore.Identity;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Repositories;
using Moeen.Shared.Requests.ParentStudent;
using Moeen.Shared.Requests.User;
using Moeen.Shared.Responses;

namespace Moeen.Api.Application.Services
{
    public class ParentStudentService : IParentStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;

        public ParentStudentService(IUnitOfWork unitOfWork, UserManager<User> userManager, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _fileService = fileService;
        }

        public async Task<GeneralResponse> LinkParentToStudentAsync(LinkParentStudentRequest request)
        {
            try
            {
                var parent = await _userManager.FindByIdAsync(request.ParentId.ToString());
                if (parent == null)
                    return GeneralResponse.NotFound("Parent not found.");

                var student = await _userManager.FindByIdAsync(request.StudentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var spec = Spec.For<ParentStudent>(ps => ps.ParentId == request.ParentId && ps.StudentId == request.StudentId);
                var existingLink = (await _unitOfWork.Repository<ParentStudent>().GetAllAsync(spec)).FirstOrDefault();

                if (existingLink != null)
                    return GeneralResponse.BadRequest("This student is already linked to this parent.");

                var parentStudent = new ParentStudent
                {
                    ParentId = request.ParentId,
                    StudentId = request.StudentId
                };

                await _unitOfWork.Repository<ParentStudent>().AddAsync(parentStudent);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Parent linked to student successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to link parent to student: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> UnlinkParentFromStudentAsync(Guid parentId, Guid studentId)
        {
            try
            {
                var spec = Spec.For<ParentStudent>(ps => ps.ParentId == parentId && ps.StudentId == studentId);
                var link = (await _unitOfWork.Repository<ParentStudent>().GetAllAsync(spec)).FirstOrDefault();

                if (link == null)
                    return GeneralResponse.NotFound("No relationship found between this parent and student.");

                await _unitOfWork.Repository<ParentStudent>().DeleteAsync(link);
                await _unitOfWork.CompleteAsync();

                return GeneralResponse.Ok("Unlinked successfully.");
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to unlink: {ex.Message}");
            }
        }

        public async Task<GeneralResponse> GetStudentsByParentIdAsync(Guid parentId)
        {
            try
            {
                var parent = await _userManager.FindByIdAsync(parentId.ToString());
                if (parent == null)
                    return GeneralResponse.NotFound("Parent not found.");

                var spec = Spec.For<ParentStudent>(ps => ps.ParentId == parentId);
                spec.AddInclude(ps => ps.Student);

                var records = await _unitOfWork.Repository<ParentStudent>().GetAllAsync(spec);

                var studentsResponse = await Task.WhenAll(records.Where(ps => ps.Student != null).Select(async ps => new UserResponse
                {
                    Id = ps.Student.Id.ToString(),
                    FullName = ps.Student.FullName,
                    Email = ps.Student.Email,
                    Phone = ps.Student.PhoneNumber,
                    ProfileImageUrl = !string.IsNullOrEmpty(ps.Student.ProfileImageUrl) ? await _fileService.GetFileUrlAsync(ps.Student.ProfileImageUrl) ?? "" : "",
                    EmailConfirmed = ps.Student.EmailConfirmed
                }));

                return GeneralResponse.Ok("Students retrieved successfully.", studentsResponse.ToList());
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve students: {ex.Message}");
            }
        }
        public async Task<GeneralResponse> GetParentsByStudentIdAsync(Guid studentId)
        {
            try
            {
                var student = await _userManager.FindByIdAsync(studentId.ToString());
                if (student == null)
                    return GeneralResponse.NotFound("Student not found.");

                var spec = Spec.For<ParentStudent>(ps => ps.StudentId == studentId);
                spec.AddInclude(ps => ps.Parent);

                var records = await _unitOfWork.Repository<ParentStudent>().GetAllAsync(spec);

                var parentsResponse = await Task.WhenAll(records.Where(ps => ps.Parent != null).Select(async ps => new UserResponse
                {
                    Id = ps.Parent.Id.ToString(),
                    FullName = ps.Parent.FullName,
                    Email = ps.Parent.Email,
                    Phone = ps.Parent.PhoneNumber,
                    ProfileImageUrl = !string.IsNullOrEmpty(ps.Parent.ProfileImageUrl) ? await _fileService.GetFileUrlAsync(ps.Parent.ProfileImageUrl) ?? "" : "",
                    EmailConfirmed = ps.Parent.EmailConfirmed
                }));

                return GeneralResponse.Ok("Parents retrieved successfully.", parentsResponse.ToList());
            }
            catch (Exception ex)
            {
                return GeneralResponse.InternalError($"Failed to retrieve parents: {ex.Message}");
            }
        }
    }
}
