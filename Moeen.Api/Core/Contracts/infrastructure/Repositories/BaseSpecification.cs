using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Moeen.Api.Core.Specifications
{
    /// <summary>
    /// تنفيذ مجرد للواجهة ISpecification، يسهل إنشاء مواصفات مخصصة بالوراثة
    /// </summary>
    public abstract class BaseSpecification<T> : ISpecification<T>
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        public List<Expression<Func<T, object>>> Includes { get; } = new();
        public List<string> IncludeStrings { get; } = new();
        public Expression<Func<T, object>> OrderBy { get; private set; }
        public Expression<Func<T, object>> OrderByDescending { get; private set; }
        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPagingEnabled { get; private set; }
        public Expression<Func<T, object>> Selector { get; private set; }
        public bool IsProjectionEnabled { get; private set; }
        public bool AsSplitQuery { get; private set; }
        public bool AsNoTracking { get; private set; }
        public bool IgnoreAutoIncludes { get; private set; }
        public bool IgnoreQueryFilters { get; private set; }

        protected BaseSpecification() { }

        protected BaseSpecification(Expression<Func<T, bool>> criteria)
        {
            Criteria = criteria;
        }

        /// <summary> إضافة Include باستخدام لامبدا (مثل u => u.Orders) </summary>
        protected void AddInclude(Expression<Func<T, object>> includeExpression)
            => Includes.Add(includeExpression);

        /// <summary> إضافة ThenInclude باستخدام مسار نصي (مثل "Orders.OrderItems") </summary>
        protected void AddThenInclude(string thenIncludePath)
            => IncludeStrings.Add(thenIncludePath);

        /// <summary> تطبيق ترتيب تصاعدي </summary>
        protected void ApplyOrderBy(Expression<Func<T, object>> orderByExpression)
            => OrderBy = orderByExpression;

        /// <summary> تطبيق ترتيب تنازلي </summary>
        protected void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescendingExpression)
            => OrderByDescending = orderByDescendingExpression;

        /// <summary> تطبيق تقسيم الصفحات (Paging) </summary>
        protected void ApplyPaging(int skip, int take)
        {
            Skip = skip;
            Take = take;
            IsPagingEnabled = true;
        }

        /// <summary> تطبيق إسقاط (اختيار أعمدة محددة فقط) </summary>
        protected void ApplyProjection(Expression<Func<T, object>> selector)
        {
            Selector = selector;
            IsProjectionEnabled = true;
        }

        /// <summary> تفعيل تقسيم الاستعلام (AsSplitQuery) لتحسين الأداء عند جلب علاقات متعددة </summary>
        protected void EnableSplitQuery() => AsSplitQuery = true;

        /// <summary> تفعيل عدم تتبع التغييرات (AsNoTracking) للقراءة فقط </summary>
        protected void EnableNoTracking() => AsNoTracking = true;

        /// <summary> تجاهل التضمينات التلقائية </summary>
        protected void EnableIgnoreAutoIncludes() => IgnoreAutoIncludes = true;

        /// <summary> تجاهل فلاتر الاستعلام العمومية (مثل IsDeleted) </summary>
        protected void EnableIgnoreQueryFilters() => IgnoreQueryFilters = true;
    }
}