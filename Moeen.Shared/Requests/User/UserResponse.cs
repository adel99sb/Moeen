using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Shared.Requests.User
{
    public class UserResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string ProfileImageUrl { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}
