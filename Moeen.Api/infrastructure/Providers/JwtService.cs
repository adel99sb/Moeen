using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Configurations;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Api.infrastructure.Providers
{
    public class JwtService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IOptions<JwtSettings> jwtSettings, UserManager<User> userManager, ILogger<JwtService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<string> GenerateTokenAsync(User user)
        {
            // 1. التحقق من صحة المستخدم
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            if (user.Id == Guid.Empty)
                throw new ArgumentException("User Id is invalid.");

            if (string.IsNullOrWhiteSpace(user.Email))
                throw new ArgumentException("User email is required.");

            // 2. التحقق من إعدادات JWT
            if (string.IsNullOrWhiteSpace(_jwtSettings.Secret) || _jwtSettings.Secret.Length < 32)
                throw new InvalidOperationException("JWT Secret must be at least 32 characters long.");

            if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
                throw new InvalidOperationException("JWT Issuer is not configured.");

            if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
                throw new InvalidOperationException("JWT Audience is not configured.");

            // 3. إنشاء الـ Claims الأساسية
            var claims = new List<Claim>
            {
                new Claim("ID", user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("Name", user.name ?? user.UserName ?? ""), // افتراض وجود Name أو UserName
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // معرّف فريد للتوكن
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };
            //error
           // 4.إضافة معرف الجامع)
            //if (user.MosqueId.HasValue && user.MosqueId.Value != Guid.Empty)
            //{
            //    claims.Add(new Claim("MosqueId", user.MosqueId.Value.ToString()));
            //}

            // 5. إضافة أدوار المستخدم (كـ Claims متعددة أو كقيمة واحدة مفصولة بفواصل)
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // 6. (اختياري) إضافة صلاحيات مخصصة إذا كنت تستخدم Permission-based authorization
            // يمكن جلب الصلاحيات من قاعدة البيانات وإضافتها كـ "permission" Claims
            // var permissions = await GetPermissionsForUserAsync(user);
            // foreach (var perm in permissions)
            // {
            //     claims.Add(new Claim("permission", perm));
            // }

            // 7. إنشاء مفتاح التوقيع
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 8. إنشاء التوكن مع صلاحية محددة
            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            // 9. كتابة التوكن كنص
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation("تم توليد توكن للمستخدم {UserId} مع صلاحية {ExpiryMinutes} دقيقة", user.Id, _jwtSettings.ExpiryMinutes);

            return tokenString;
        }
    }
}