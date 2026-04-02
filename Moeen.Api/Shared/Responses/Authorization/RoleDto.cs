
using System;
using System.Collections.Generic;

namespace Moeen.Api.Shared.Responses.Authorization
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; }
    }
}