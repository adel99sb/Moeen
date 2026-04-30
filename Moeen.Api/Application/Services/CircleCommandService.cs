using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Shared.Requests.Circle;
using Moeen.Shared.Responses.Circle;

namespace Moeen.Api.Application.Services
{
    public class CircleCommandService : ICircleCommandService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CircleCommandService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CircleDto> CreateCircleAsync(CreateCircleRequest request)
        {
            // تحقق من وجود الفوج
            var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.FoujId);
            if (fouj == null)
                throw new ArgumentException("Fouj not found.", nameof(request.FoujId));

            // تحقق من وجود المعلم
            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId);
            if (teacher == null)
                throw new ArgumentException("Teacher not found.", nameof(request.TeacherId));

            var halqa = new Halqa
            {
                Id = Guid.NewGuid(),
                FoujId = request.FoujId,
                Name = request.Name,
                TeacherId = request.TeacherId,
               Type = request.Type
            };

            await _unitOfWork.Repository<Halqa>().AddAsync(halqa);
            await _unitOfWork.CompleteAsync();

            return new CircleDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj?.name,
                TeacherId = halqa.TeacherId,
                TeacherName = teacher?.name,
                Type = halqa.Type,
                StudentsCount = 0
            };
        }

        public async Task<bool> DeleteCircleAsync(DeleteCircleRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                return false;

            await _unitOfWork.Repository<Halqa>().DeleteAsync(halqa);
            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<CircleDto> MoveToFoujAsync(MoveCircleToFoujRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.CircleId));

            var targetFouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.TargetFoujId);
            if (targetFouj == null)
                throw new ArgumentException("Target fouj not found.", nameof(request.TargetFoujId));

            halqa.FoujId = request.TargetFoujId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(halqa.TeacherId);

            return new CircleDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = targetFouj?.name,
                TeacherId = halqa.TeacherId,
                TeacherName = teacher?.name,
                Type = halqa.Type
            };
        }

        public async Task<CircleDto> ReassignTeacherAsync(ReassignCircleTeacherRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.CircleId));

            var newTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.NewTeacherId);
            if (newTeacher == null)
                throw new ArgumentException("New teacher not found.", nameof(request.NewTeacherId));

            halqa.TeacherId = request.NewTeacherId;
            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(halqa.FoujId);

            return new CircleDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj?.name,
                TeacherId = halqa.TeacherId,
                TeacherName = newTeacher?.name,
                Type = halqa.Type
            };
        }

        public async Task<CircleDto> UpdateCircleAsync(UpdateCircleRequest request)
        {
            var halqa = await _unitOfWork.Repository<Halqa>().GetByIdAsync(request.CircleId);
            if (halqa == null)
                throw new ArgumentException("Circle not found.", nameof(request.CircleId));

            if (!string.IsNullOrWhiteSpace(request.Name))
                halqa.Name = request.Name;

            if (!string.IsNullOrWhiteSpace(request.Type))
                halqa.type = request.Type;

            if (request.FoujId.HasValue)
            {
                var fouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(request.FoujId.Value);
                if (fouj == null)
                    throw new ArgumentException("Target fouj not found.", nameof(request.FoujId));
                halqa.FoujId = request.FoujId.Value;
            }

            if (request.TeacherId.HasValue)
            {
                var teacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(request.TeacherId.Value);
                if (teacher == null)
                    throw new ArgumentException("Teacher not found.", nameof(request.TeacherId));
                halqa.TeacherId = request.TeacherId.Value;
            }

            await _unitOfWork.Repository<Halqa>().UpdateAsync(halqa);
            await _unitOfWork.CompleteAsync();

            var resultFouj = await _unitOfWork.Repository<Fouj>().GetByIdAsync(halqa.FoujId);
            var resultTeacher = await _unitOfWork.Repository<Teacher>().GetByIdAsync(halqa.TeacherId);

            return new CircleDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = resultFouj?.name,
                TeacherId = halqa.TeacherId,
                TeacherName = resultTeacher?.name,
                Type = halqa.Type
            };
        }
    }
}
