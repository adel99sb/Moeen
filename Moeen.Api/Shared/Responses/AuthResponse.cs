//namespace Moeen.Api.Shared.Responses;
//public class AuthResponse
//{
//    public bool IsSuccess { get; set; }
//    public string Message { get; set; } = string.Empty;
//    public string Token { get; set; } = string.Empty;
//}
using Moeen.Api.Shared.Responses.Identity;
using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses
{
    public class AuthResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public List<string> Roles { get; set; }
        public string ErrorMessage { get; set; } // في حال فشل التسجيل
    }
}
