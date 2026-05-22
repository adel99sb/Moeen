using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
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

        public FoujService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<GeneralResponse> CreateFoujAsync(CreateFoujRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("»Ì«‰«  «·›ÊÃ „ÿ·Ê»….");

            var mosqueExists = await _context.Mosques.AnyAsync(m => m.Id == request.MosqueId);
            if (!mosqueExists)
                return GeneralResponse.NotFound("«·„”Ãœ €Ì— „ÊÃÊœ.");

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

            return GeneralResponse.Ok(" „ ≈‰‘«¡ «·›ÊÃ »‰Ã«Õ.", MapFouj(fouj, 0));
        }

        public async Task<GeneralResponse> UpdateFoujAsync(UpdateFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·›ÊÃ „ÿ·Ê».");

            var fouj = await _context.Foujs
                .Include(f => f.Halqas)
                .FirstOrDefaultAsync(f => f.Id == request.FoujId);

            if (fouj == null)
                return GeneralResponse.NotFound("«·›ÊÃ €Ì— „ÊÃÊœ.");

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
                    return GeneralResponse.NotFound("«·„”Ãœ €Ì— „ÊÃÊœ.");

                fouj.MosqueId = request.MosqueId.Value;
            }

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „  ÕœÌÀ «·›ÊÃ »‰Ã«Õ.", MapFouj(fouj, fouj.Halqas?.Count ?? 0));
        }

        public async Task<GeneralResponse> DeleteFoujAsync(DeleteFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·›ÊÃ „ÿ·Ê».");

            var fouj = await _context.Foujs
                .Include(f => f.Halqas)
                .FirstOrDefaultAsync(f => f.Id == request.FoujId);

            if (fouj == null)
                return GeneralResponse.NotFound("«·›ÊÃ €Ì— „ÊÃÊœ.");

            if (fouj.Halqas?.Any() == true)
                return GeneralResponse.BadRequest("·« Ì„ﬂ‰ Õ–› «·›ÊÃ ·ÊÃÊœ Õ·ﬁ«  „— »ÿ….");

            _context.Foujs.Remove(fouj);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „ Õ–› «·›ÊÃ »‰Ã«Õ.");
        }

        public async Task<GeneralResponse> GetAllFoujsAsync(GetAllFoujsRequest request)
        {
            request ??= new GetAllFoujsRequest();

            var query = _context.Foujs
                .AsNoTracking()
                .Include(f => f.Halqas)
                .AsQueryable();

            if (request.MosqueId.HasValue)
                query = query.Where(f => f.MosqueId == request.MosqueId.Value);

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

            return GeneralResponse.Ok(" „ Ã·» ﬁ«∆„… «·√›Ê«Ã.", foujs);
        }

        public async Task<GeneralResponse> AddHalqaToFoujAsync(AddHalqaToFoujRequest request)
        {
            if (request == null || request.FoujId == Guid.Empty || request.HalqaId == Guid.Empty)
                return GeneralResponse.BadRequest("„⁄—› «·›ÊÃ Ê«·Õ·ﬁ… „ÿ·Ê»«‰.");

            var fouj = await _context.Foujs.FindAsync(request.FoujId);
            if (fouj == null)
                return GeneralResponse.NotFound("«·›ÊÃ €Ì— „ÊÃÊœ.");

            var halqa = await _context.Halqas
                .Include(h => h.Teacher)
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId);

            if (halqa == null)
                return GeneralResponse.NotFound("«·Õ·ﬁ… €Ì— „ÊÃÊœ….");

            halqa.FoujId = request.FoujId;
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „  ≈÷«›… «·Õ·ﬁ… ≈·Ï «·›ÊÃ.", new HalqaDto
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
                return GeneralResponse.BadRequest("„⁄—› «·›ÊÃ Ê«·Õ·ﬁ… „ÿ·Ê»«‰.");

            var halqa = await _context.Halqas
                .FirstOrDefaultAsync(h => h.Id == request.HalqaId && h.FoujId == request.FoujId);

            if (halqa == null)
                return GeneralResponse.NotFound("«·Õ·ﬁ… €Ì— „ÊÃÊœ… œ«Œ· «·›ÊÃ.");

            _context.Halqas.Remove(halqa);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok(" „  ≈“«·… «·Õ·ﬁ… „‰ «·›ÊÃ.");
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