using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Authorization
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<string> Permissions { get; set; } = new List<string>();
    }
}