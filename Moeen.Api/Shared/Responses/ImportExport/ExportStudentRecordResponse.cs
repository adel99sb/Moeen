namespace Moeen.Api.Shared.Responses.ImportExport
{
    public class ExportStudentRecordResponse
    {
        public byte[] FileContent { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; } // مثال: "application/pdf", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    }
}