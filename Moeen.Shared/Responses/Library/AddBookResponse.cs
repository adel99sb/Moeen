using System;

namespace Moeen.Shared.Responses.Library
{
    public class AddBookResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? BookId { get; set; }
    }
}