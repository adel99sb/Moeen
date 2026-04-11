using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moeen.Api.Core.Specifications
{
    /// <summary>
    /// واجهة تحدد مكونات الـ Specification (المواصفة) للاستعلامات المعقدة
    /// </summary>
    /// <typeparam name="T">نوع الكيان</typeparam>
    public interface ISpecification<T>
    {
        /// <summary> شرط التصفية الأساسي (WHERE) </summary>
        Expression<Func<T, bool>> Criteria { get; }

        /// <summary> قائمة العلاقات المراد تحميلها باستخدام Include اللامبدا </summary>
        List<Expression<Func<T, object>>> Includes { get; }

        /// <summary> قائمة المسارات النصية للعلاقات المتداخلة (مثل "Orders.OrderItems") </summary>
        List<string> IncludeStrings { get; }

        /// <summary> ترتيب تصاعدي </summary>
        Expression<Func<T, object>> OrderBy { get; }

        /// <summary> ترتيب تنازلي </summary>
        Expression<Func<T, object>> OrderByDescending { get; }

        /// <summary> عدد السجلات المطلوبة (Take) </summary>
        int Take { get; }

        /// <summary> عدد السجلات المطلوب تجاوزها (Skip) </summary>
        int Skip { get; }

        /// <summary> هل تم تفعيل التقسيم إلى صفحات؟ </summary>
        bool IsPagingEnabled { get; }

        /// <summary> خاصية الإسقاط (اختيار أعمدة معينة) </summary>
        Expression<Func<T, object>> Selector { get; }

        /// <summary> هل تم تفعيل الإسقاط؟ </summary>
        bool IsProjectionEnabled { get; }

        /// <summary> تحسين الأداء: تقسيم الاستعلام (AsSplitQuery) </summary>
        bool AsSplitQuery { get; }

        /// <summary> تحسين الأداء: عدم تتبع التغييرات (AsNoTracking) </summary>
        bool AsNoTracking { get; }

        /// <summary> تجاهل التضمينات التلقائية (IgnoreAutoIncludes) </summary>
        bool IgnoreAutoIncludes { get; }

        /// <summary> تجاهل فلاتر الاستعلام العمومية (IgnoreQueryFilters) مثل Soft Delete </summary>
        bool IgnoreQueryFilters { get; }
    }
}