using System.Linq;
using Microsoft.EntityFrameworkCore;
using Moeen.Api.Core.Specifications;

namespace Moeen.Infrastructure.Specifications
{
    /// <summary>
    /// مسؤول عن تطبيق المواصفة على IQueryable وتحويلها إلى استعلام قابل للتنفيذ
    /// </summary>
    public class SpecificationEvaluator<T> where T : class
    {
        /// <summary>
        /// تطبيق المواصفة بالكامل (Includes, Where, OrderBy, Paging, Projection, إلخ)
        /// </summary>
        public virtual IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> spec)
        {
            var query = inputQuery;

            // تطبيق شرط التصفية
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);

            // تطبيق Includes اللامبدا
            foreach (var includeString in spec.Includes)
                query = query.Include(include);

            // تطبيق ThenIncludes النصية (للمسارات المتداخلة)
            foreach (var includeString in spec.IncludeStrings)
                query = query.Include(includeString);

            // تطبيق الترتيب (إجباري إذا كان هناك Paging)
            if (spec.OrderBy != null)
                query = query.OrderBy(spec.OrderBy);
            else if (spec.OrderByDescending != null)
                query = query.OrderByDescending(spec.OrderByDescending);
            else if (spec.IsPagingEnabled)
                throw new InvalidOperationException("Paging requires an OrderBy to ensure stable results.");

            // تطبيق الإسقاط (اختيار أعمدة محددة) – ملاحظة: قد يحتاج إلى تحويل النوع
            if (spec.IsProjectionEnabled && spec.Selector != null)
                query = query.Select(spec.Selector).Cast<T>();

            // تحسينات الأداء
            if (spec.AsSplitQuery)
                query = query.AsSplitQuery();
            if (spec.AsNoTracking)
                query = query.AsNoTracking();
            if (spec.IgnoreAutoIncludes)
                query = query.IgnoreAutoIncludes();
            if (spec.IgnoreQueryFilters)
                query = query.IgnoreQueryFilters();

            // تطبيق الصفحات (Skip/Take)
            if (spec.IsPagingEnabled)
                query = query.Skip(spec.Skip).Take(spec.Take);

            return query;
        }

        /// <summary>
        /// حساب عدد السجلات التي تحقق المواصفة (قبل تطبيق Skip/Take)
        /// </summary>
        public virtual async Task<int> CountAsync(IQueryable<T> inputQuery, ISpecification<T> spec, CancellationToken cancellationToken = default)
        {
            var query = inputQuery;
            if (spec.Criteria != null)
                query = query.Where(spec.Criteria);
            return await query.CountAsync(cancellationToken);
        }
    }
}