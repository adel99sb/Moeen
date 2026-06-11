using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Moeen.Api.Core.Contracts.infrastructure.Providers;
using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using Moeen.Api.infrastructure.Configurations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Moeen.Api.infrastructure.Providers
{
    public class JwtService : IJwtService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileService _fileService;

        public JwtService(IUnitOfWork unitOfWork, IFileService fileService)
        {
            _unitOfWork = unitOfWork;
            _fileService = fileService;
        }

        public async Task<string> GenerateJwtToken(User user, UserManager<User> userManager)
        {
            try
            {
                var jwtSettings = AppSettings.Instance.JwtSettings; // Access singleton config instance                
                var claims = new List<Claim>
                {
                    new Claim("UserIdentifier", user.Id.ToString()),
                    new Claim("FullName", user.FullName),
                    new Claim("Email", user.Email == null ? string.Empty : user.Email),
                    new Claim("EmailConfirmed",user.EmailConfirmed.ToString()),
                    new Claim("Phone",user.PhoneNumber)
                };
                var imagePath = await _fileService.GetFileUrlAsync(user.ProfileImageUrl);
                if (user.ProfileImageUrl != null)
                    claims.Add(new Claim("ImagePath", imagePath));
                else
                    claims.Add(new Claim("ImagePath", string.Empty));


                var roles = await userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiryMinutes),
                    signingCredentials: creds);
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            catch (Exception)
            {

                throw new Exception("Generate token faild");
            }

        }
    }
}