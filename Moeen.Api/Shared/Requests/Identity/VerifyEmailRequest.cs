namespace Moeen.Api.Shared.Requests.Identity
{
  
        public class VerifyEmailRequest
        {
            public string? Code { get; set; }
            public string? Email { get; set; }
            public Guid? UserId { get; set; }
        }
    }
