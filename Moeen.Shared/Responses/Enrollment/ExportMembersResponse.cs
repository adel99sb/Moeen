namespace Moeen.Shared.Responses.Enrollment
{
    public class ExportMembersResponse
    {
        public byte[] FileContent { get; set; }
        public string ContentType { get; set; } = string.Empty; // "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "application/pdf", etc.
        public string FileName { get; set; } = string.Empty;
    }
}