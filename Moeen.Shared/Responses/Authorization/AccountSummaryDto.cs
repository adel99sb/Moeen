using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Authorization
{
    public class AccountSummaryDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? MosqueName { get; set; }
        public string? FoujName { get; set; }
        public bool IsActive { get; set; }
        public string StatusLabel => IsActive ? "‰‘ÿ" : "€Ì— ‰‘ÿ";

        // ··√Â«·Ì: √”„«¡ «·√»‰«¡
        public List<string>? ChildrenNames { get; set; }
    }
}