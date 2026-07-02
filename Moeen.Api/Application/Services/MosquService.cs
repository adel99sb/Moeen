using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Contracts.Application;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Data;
using Moeen.Shared.Requests.Mosuq;
using Moeen.Shared.Responses;
using Moeen.Shared.Responses.Halqa;
using Moeen.Shared.Responses.Enrollment;
using Moeen.Shared.Responses.Mosuq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Moeen.Api.Application.Services
{
    public class MosquService : IMosquService
    {
        private readonly AppDbContext _context;
        private readonly ICurrentUserService? _currentUserService;

        public MosquService(AppDbContext context, ICurrentUserService? currentUserService = null)
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

        public async Task<GeneralResponse> AddMosqu(AddMosquReq req)
        {
            if (req == null)
                return GeneralResponse.BadRequest("بيانات المسجد غير صالحة.");

            var mosque = new Mosque
            {
                Id = Guid.NewGuid(),
                name = req.name,
                address = req.address,
                contact_phone = req.contact_phone,
                Description = req.Description,
                Latitude = req.Latitude,
                Longitude = req.Longitude
            };

            await _context.Mosques.AddAsync(mosque);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إضافة المسجد بنجاح.", MapMosqueDto(mosque));
        }

        public async Task<GeneralResponse> GetAllMosqus(GetAllMosqusRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var query = _context.Mosques.AsQueryable();
            var managedMosqueId = await ResolveManagedMosqueIdAsync();
            if (managedMosqueId.HasValue)
                query = query.Where(m => m.Id == managedMosqueId.Value);

            var totalCount = await query.CountAsync();

            var mosqus = await query
                .OrderBy(m => m.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new MosquDto
                {
                    Id = m.Id,
                    name = m.name,
                    address = m.address,
                    contactPhone = m.contact_phone,
                    Description = m.Description,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
                })
                .ToListAsync();

            return GeneralResponse.Ok("تم جلب المساجد بنجاح.", mosqus, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetMosqueByIdAsync(GetMosqueByIdRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            return GeneralResponse.Ok("تم جلب بيانات المسجد.", MapMosqueDto(mosque));
        }

        public async Task<GeneralResponse> GetCirclesByMosqueAsync(GetCirclesByMosqueRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var baseQuery = _context.Halqas
                .Include(h => h.Fouj)
                .Include(h => h.Teacher)
                .Where(h => h.Fouj.MosqueId == request.MosqueId);

            var totalCount = await baseQuery.CountAsync();

            var halqas = await baseQuery
                .OrderBy(h => h.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var halqaIds = halqas.Select(h => h.Id).ToList();

            var studentsCounts = await _context.ProgressEntries
                .Where(pe => halqaIds.Contains(pe.HalqaId))
                .GroupBy(pe => pe.HalqaId)
                .Select(g => new { HalqaId = g.Key, Count = g.Select(x => x.StudentId).Distinct().Count() })
                .ToListAsync();

            var countMap = studentsCounts.ToDictionary(x => x.HalqaId, x => x.Count);

            var result = halqas.Select(h => new HalqaDto
            {
                Id = h.Id,
                Name = h.Name,
                FoujId = h.FoujId,
                FoujName = h.Fouj?.name ?? string.Empty,
                TeacherId = h.TeacherId ?? Guid.Empty,
                TeacherName = h.Teacher?.name ?? string.Empty,
                Type = h.Type,
                StudentsCount = countMap.TryGetValue(h.Id, out var c) ? c : 0
            }).ToList();

            return GeneralResponse.Ok("تم جلب الحلقات بنجاح.", result, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetTeachersByMosqueAsync(GetTeachersByMosqueRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var page = Math.Max(1, request.Page);
            var pageSize = Math.Max(1, request.PageSize);

            var baseQuery = _context.Teachers
                .Include(t => t.Mosque)
                .Where(t => t.MosqueId == request.MosqueId && t.status == 0);

            var totalCount = await baseQuery.CountAsync();

            var teachers = await baseQuery
                .OrderBy(t => t.name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = teachers.Select(t => new TeacherDto
            {
                Id = t.Id,
                Name = t.name,
                Email = t.Email,
                Phone = t.PhoneNumber,
                Gender = t.gender,
                FontSize = t.font_size,
                Role = t.role,
                Status = t.status,
                Theme = t.theme,
                ProfileImageUrl = t.profile_imageUrl,
                CreatedAt = t.created_at,
                JoinedAt = t.JoinedAt,
                MosqueId = t.MosqueId,
                Bio = t.Bio,
                AssignedAt = t.assigned_at,
                MosqueName = t.Mosque?.name ?? string.Empty
            }).ToList();

            return GeneralResponse.Ok("تم جلب المعلمين بنجاح.", result, page, pageSize, totalCount);
        }

        public async Task<GeneralResponse> GetMosqueStatisticsAsync(GetMosqueStatisticsRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var foujIds = await _context.Foujs
                .Where(f => f.MosqueId == request.MosqueId)
                .Select(f => f.Id)
                .ToListAsync();

            var circlesCount = await _context.Halqas.CountAsync(h => foujIds.Contains(h.FoujId));
            var studentsCount = await _context.Students.CountAsync(s => s.MosqueId == request.MosqueId);
            var teachersCount = await _context.Teachers.CountAsync(t => t.MosqueId == request.MosqueId && t.status == 0);

            var dto = new MosqueStatisticsDto
            {
                CirclesCount = circlesCount,
                StudentsCount = studentsCount,
                TeachersCount = teachersCount,
                Capacity = 0,
                OccupancyRate = 0
            };

            return GeneralResponse.Ok("تم جلب إحصائيات المسجد.", dto);
        }

        public async Task<GeneralResponse> GetNearbyMosquesAsync(GetNearbyMosquesRequest request)
        {
            if (request == null)
                return GeneralResponse.BadRequest("طلب غير صالح.");

            var safeCos = Math.Max(0.0001, Math.Cos(DegreesToRadians(request.Latitude)));
            var latDelta = request.RadiusKm / 111.0;
            var lonDelta = request.RadiusKm / (111.0 * safeCos);

            var minLat = request.Latitude - latDelta;
            var maxLat = request.Latitude + latDelta;
            var minLon = request.Longitude - lonDelta;
            var maxLon = request.Longitude + lonDelta;

            var candidates = await _context.Mosques
                .Where(m => m.Latitude >= minLat && m.Latitude <= maxLat &&
                            m.Longitude >= minLon && m.Longitude <= maxLon)
                .Select(m => new MosqueDto
                {
                    Id = m.Id,
                    Name = m.name,
                    Address = m.address,
                    ContactPhone = m.contact_phone,
                    Description = m.Description,
                    Capacity = 0,
                    Latitude = m.Latitude,
                    Longitude = m.Longitude
                })
                .ToListAsync();

            var result = candidates
                .Where(m => HaversineKm(request.Latitude, request.Longitude, m.Latitude, m.Longitude) <= request.RadiusKm)
                .ToList();

            return GeneralResponse.Ok("تم جلب المساجد القريبة.", result);
        }

        public async Task<GeneralResponse> UpdateMosqueInfoAsync(UpdateMosqueInfoRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            if (!string.IsNullOrWhiteSpace(request.Name))
                mosque.name = request.Name;
            if (!string.IsNullOrWhiteSpace(request.Address))
                mosque.address = request.Address;
            if (!string.IsNullOrWhiteSpace(request.ContactPhone))
                mosque.contact_phone = request.ContactPhone;
            if (!string.IsNullOrWhiteSpace(request.Description))
                mosque.Description = request.Description;
            if (request.Latitude.HasValue)
                mosque.Latitude = request.Latitude.Value;
            if (request.Longitude.HasValue)
                mosque.Longitude = request.Longitude.Value;

            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تحديث بيانات المسجد.", MapMosqueDto(mosque));
        }

        public async Task<GeneralResponse> AssignMosqueAdminAsync(AssignMosqueAdminRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty || request.AdminUserId == Guid.Empty)
                return GeneralResponse.BadRequest("بيانات غير صالحة.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var supervisor = await _context.Supervisors.FirstOrDefaultAsync(s => s.Id == request.AdminUserId);
            if (supervisor == null)
                return GeneralResponse.BadRequest("المستخدم ليس مشرفاً مسجلاً في النظام.");

            supervisor.MosqueId = request.MosqueId;
            supervisor.assigned_at = DateTime.UtcNow;

            _context.Supervisors.Update(supervisor);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم تعيين المشرف بنجاح.", true);
        }

        public async Task<GeneralResponse> DeleteMosqueAsync(DeleteMosqueRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var mosque = await _context.Mosques.FindAsync(request.MosqueId);
            if (mosque == null)
                return GeneralResponse.NotFound("المسجد غير موجود.");

            var foujIds = await _context.Foujs
                .Where(f => f.MosqueId == request.MosqueId)
                .Select(f => f.Id)
                .ToListAsync();

            var hasRelations =
                foujIds.Any() ||
                await _context.Halqas.AnyAsync(h => foujIds.Contains(h.FoujId)) ||
                await _context.Teachers.AnyAsync(t => t.MosqueId == request.MosqueId) ||
                await _context.Students.AnyAsync(s => s.MosqueId == request.MosqueId) ||
                await _context.Supervisors.AnyAsync(s => s.MosqueId == request.MosqueId);

            if (hasRelations)
                return GeneralResponse.BadRequest("لا يمكن حذف المسجد لوجود بيانات مرتبطة به.");

            _context.Mosques.Remove(mosque);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم حذف المسجد بنجاح.", true);
        }

        public async Task<GeneralResponse> UnassignMosqueAdminAsync(UnassignMosqueAdminRequest request)
        {
            if (request == null || request.MosqueId == Guid.Empty)
                return GeneralResponse.BadRequest("معرّف المسجد غير صالح.");

            var supervisors = await _context.Supervisors
                .Where(s => s.MosqueId == request.MosqueId)
                .ToListAsync();

            if (supervisors.Count == 0)
                return GeneralResponse.NotFound("لا يوجد مشرف معيّن لهذا المسجد.");

            _context.Supervisors.RemoveRange(supervisors);
            await _context.SaveChangesAsync();

            return GeneralResponse.Ok("تم إلغاء تعيين المشرف.", true);
        }

        private static MosqueDto MapMosqueDto(Mosque mosque)
        {
            return new MosqueDto
            {
                Id = mosque.Id,
                Name = mosque.name,
                Address = mosque.address,
                ContactPhone = mosque.contact_phone,
                Description = mosque.Description,
                Capacity = 0,
                Latitude = mosque.Latitude,
                Longitude = mosque.Longitude
            };
        }

        private static double HaversineKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Asin(Math.Min(1, Math.Sqrt(a)));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180.0);
    }
}

