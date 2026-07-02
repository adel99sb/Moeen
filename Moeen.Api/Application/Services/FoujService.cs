using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Fouj;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Fouj;
using Moeen.Shared.Responses.Halqa;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class FoujService : IFoujService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService? _currentUserService;

        public FoujService(AppDbContext context, ICurrentUserService? currentUserService = null)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        private async Task<Guid?> ResolveManagedMosqueIdAsync()
        {
            var currentUserId = _currentUserService?.CurrentUserId;
            if (!currentUserId.HasValue)
                return null;

            return await _context.Supervisors
                .AsNoTracking()
                .Where(s => s.Id == currentUserId.Value)
                .Select(s => (Guid?)s.MosqueId)
                .FirstOrDefaultAsync();
        }

        private static Guid? ResolveEffectiveMosqueId(Guid? managedMosqueId, Guid? requestedMosqueId)
            => managedMosqueId ?? requestedMosqueId;

        public async Task<GeneralResponse> CreateFoujAsync(CreateFoujRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("������ ����� ������.");

            var mosqueExists = await _context.Mosques.AnyAsync(m => m.Id == request.MosqueId);
            if (!mosqueExists)
                return GeneralResponse.NotFound("������ ��� �����.");

            var fouj = new Fouj
            {
                Id = Guid.NewGuid(),
                name = request.Name,
                MosqueId = request.MosqueId,
                start_time = request.StartTime,
                End_time = request.EndTime
            };

            await _context.Foujs.AddAsync(fouj);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("�� ����� ����� �����.", MapFouj(fouj, 0));
        }

        public async Task<GeneralResponse> UpdateFoujAsync(UpdateFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty)
                return GeneralResponse.BadRequest("���� ����� �����.");

            var fouj = await _context.Foujs
                .Include(f => f.Halqas)
                .FirstOrDefaultAsync(f => f.Id == request.FoujId);

            if (fouj == null)
                return GeneralResponse.NotFound("����� ��� �����.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                fouj.name = request.Name;

            if (request.StartTime.HasValue)
                fouj.start_time = request.StartTime.Value;

            if (request.EndTime.HasValue)
                fouj.End_time = request.EndTime.Value;

            if (request.MosqueId.HasValue)
            {
                var mosqueExists = await _context.Mosques.AnyAsync(m => m.Id == request.MosqueId.Value);
                if (!mosqueExists)
                    return GeneralResponse.NotFound("������ ��� �����.");

                fouj.MosqueId = request.MosqueId.Value;
            }

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("�� ����� ����� �����.", MapFouj(fouj, fouj.Halqas?.Count ?? 0));
        }

        public async Task<GeneralResponse> DeleteFoujAsync(DeleteFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty)
                return GeneralResponse.BadRequest("���� ����� �����.");

            var fouj = await _context.Foujs
                .Include(f => f.Halqas)
                .FirstOrDefaultAsync(f => f.Id == request.FoujId);

            if (fouj == null)
                return GeneralResponse.NotFound("����� ��� �����.");

            if (fouj.Halqas?.Any() == true)
                return GeneralResponse.BadRequest("�� ���� ��� ����� ����� ����� ������.");

            _context.Foujs.Remove(fouj);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("�� ��� ����� �����.");
        }

        public async Task<GeneralResponse> GetAllFoujsAsync(GetAllFoujsRequest request)
        {
            request ??= new GetAllFoujsRequest();

            var query = _context.Foujs
                .AsNoTracking()
                .Include(f => f.Halqas)
                .AsQueryable();

            var effectiveMosqueId = ResolveEffectiveMosqueId(await ResolveManagedMosqueIdAsync(), request.MosqueId);
            if (effectiveMosqueId.HasValue)
                query = query.Where(f => f.MosqueId == effectiveMosqueId.Value);

            var foujs = await query
                .OrderBy(f => f.name)
                .Select(f => new FoujDto
                {
                    Id = f.Id,
                    Name = f.name,
                    MosqueId = f.MosqueId,
                    StartTime = f.start_time,
                    EndTime = f.End_time,
                    HalqasCount = f.Halqas.Count
                })
                .ToListAsync();

            return GeneralResponse.Ok("�� ��� ����� �������.", foujs);
        }

        public async Task<GeneralResponse> AddHalqaToFoujAsync(AddHalqaToFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty || request.HalqaId == Guid.Empty)
                return GeneralResponse.BadRequest("���� ����� ������� �������.");

            var fouj = await _context.Foujs.FindAsync(request.FoujId);
            if (fouj == null)
                return GeneralResponse.NotFound("����� ��� �����.");

            var halqa = await _context.Halqas
                .Include(h => h.Teacher)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("������ ��� ������.");

            halqa.FoujId = request.FoujId;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("��� ����� ������ ��� �����.", new HalqaDto
            {
                Id = halqa.Id,
                Name = halqa.Name,
                FoujId = halqa.FoujId,
                FoujName = fouj.name,
                TeacherId = halqa.TeacherId ?? Guid.Empty,
                TeacherName = halqa.Teacher?.name,
                Type = halqa.Type,
                StudentsCount = 0
            });
        }

        public async Task<GeneralResponse> RemoveHalqaFromFoujAsync(RemoveHalqaFromFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty || request.HalqaId == Guid.Empty)
                return GeneralResponse.BadRequest("���� ����� ������� �������.");

            var halqa = await _context.Halqas
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId && h.FoujId == request.FoujId);

            if (halqa == null)
                return GeneralResponse.NotFound("������ ��� ������ ���� �����.");

            _context.Halqas.Remove(halqa);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("��� ����� ������ �� �����.");
        }

        private static FoujDto MapFouj(Fouj fouj, int halqasCount)
        {
            return new FoujDto
            {
                Id = fouj.Id,
                Name = fouj.name,
                MosqueId = fouj.MosqueId,
                StartTime = fouj.start_time,
                EndTime = fouj.End_time,
                HalqasCount = halqasCount
            };
        }
    }
}