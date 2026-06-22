using System;
using System.Collections.Generic;

namespace Moeen.Shared.Responses.Mobile
{
    public class ParentProgressResponse
    {
        public Guid ParentId { get; set; }
        public string ParentName { get; set; } = string.Empty;
        public Guid? SelectedStudentId { get; set; }
        public string SelectedStudentName { get; set; } = string.Empty;
        public List<ParentChildDto> Children { get; set; } = new();
        public StudentProgressResponse Progress { get; set; } = new();
    }
}
