namespace Moeen.Shared.Responses.QuranCurriculum
{
    public class PageDto
    {
        public int PageNumber { get; set; }        // رقم الصفحة (1-604)
        public int JuzNumber { get; set; }          // رقم الجزء التابعة له
        public string SurahName { get; set; }        // اسم السورة (اختياري)
        public int VerseRange { get; set; }          // نطاق الآيات (اختياري)
    }
}