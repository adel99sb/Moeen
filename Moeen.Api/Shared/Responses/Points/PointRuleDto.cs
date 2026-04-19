using Moeen.Api.Core.Constants;
using System;

namespace Moeen.Api.Shared.Responses.Points
{
    public class PointRuleDto
    {
        public Guid Id { get; set; }
        public Grade Grade { get; set; }
        public int Points { get; set; }
        public bool IsActive { get; set; }
    }
}