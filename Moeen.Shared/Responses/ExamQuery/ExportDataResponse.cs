namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExportDataResponse
    {
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] Content { get; set; }
    }
}