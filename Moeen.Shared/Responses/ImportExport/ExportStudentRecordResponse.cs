namespace Moeen.Shared.Responses.ImportExport
{
    public class ExportStudentRecordResponse
    {
        public byte[] FileContent { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty; // مثال: "application/pdf", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    }
}