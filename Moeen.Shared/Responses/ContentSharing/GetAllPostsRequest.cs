using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moeen.Shared.Responses.ContentSharing
{
    internal class GetAllPostsRequest
    {
        public string? Search { get; set; } // نص البحث
        public int Page { get; set; } = 1; // رقم الصفحة
        public int PageSize { get; set; } = 20; // حجم الصفحة الافتراضي
        public string? OrderBy { get; set; } // ترتيب (مثلاً: "CreatedAt", "InteractionsCount")
        public bool Descending { get; set; } = true; // تنازلي أو تصاعدي
        public Guid? MosqueId { get; set; } // تصفية حسب مسجد
        public Guid? UserId { get; set; } // تصفية حسب المستخدم (الناشر)
    }
}
