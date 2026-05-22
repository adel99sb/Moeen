namespace Moeen.Shared.Responses.ExamQuery
{
    public class ExportDataResponse
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Content { get; set; }
    }
}