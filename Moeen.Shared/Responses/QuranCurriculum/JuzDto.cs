namespace Moeen.Shared.Responses.QuranCurriculum
{
    public class JuzDto
    {
        public int Number { get; set; }          // رقم الجزء (1-30)
        public int PageCount { get; set; }        // عدد صفحات الجزء
        public int StartPage { get; set; }        // صفحة البداية (اختياري)
        public int EndPage { get; set; }          // صفحة النهاية (اختياري)
        public string Name { get; set; } = string.Empty;           // اسم الجزء (مثلاً "جزء عم")
    }
}