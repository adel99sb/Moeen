using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Identity
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
        public UserDto User { get; set; }
        public List<string> Roles { get; set; }
        public string ErrorMessage { get; set; }
        public bool IsSuccess { get; internal set; }
        public string Message { get; internal set; }
    }
}