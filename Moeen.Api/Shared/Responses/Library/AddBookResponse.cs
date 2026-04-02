using System;

namespace Moeen.Api.Shared.Responses.Library
{
    public class AddBookResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public Guid? BookId { get; set; }
    }
}