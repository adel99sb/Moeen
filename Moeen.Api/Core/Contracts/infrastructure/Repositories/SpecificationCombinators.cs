using System;
using System.Linq.Expressions;

namespace Moeen.Api.Core.Specifications
{
    /// <summary>
    /// دوال مساعدة لدمج المواصفات باستخدام And, Or, Not
    /// </summary>
    public static class SpecificationCombinators
    {
        /// <summary> دمج مواصفتين بعلاقة AND </summary>
        public static BaseSpecification<T> And<T>(this BaseSpecification<T> left, BaseSpecification<T> right)
        {
            var combinedCriteria = CombineExpressions(left.Criteria, right.Criteria, Expression.AndAlso);
            return new CombinedSpecification<T>(combinedCriteria);
        }

        /// <summary> دمج مواصفتين بعلاقة OR </summary>
        public static BaseSpecification<T> Or<T>(this BaseSpecification<T> left, BaseSpecification<T> right)
        {
            var combinedCriteria = CombineExpressions(left.Criteria, right.Criteria, Expression.OrElse);
            return new CombinedSpecification<T>(combinedCriteria);
        }

        /// <summary> عكس شرط المواصفة (NOT) </summary>
        public static BaseSpecification<T> Not<T>(this BaseSpecification<T> spec)
        {
            if (spec.Criteria == null)
                throw new InvalidOperationException("Cannot apply NOT on a specification without Criteria.");
            var notCriteria = Expression.Lambda<Func<T, bool>>(
                Expression.Not(spec.Criteria.Body), spec.Criteria.Parameters);
            return new CombinedSpecification<T>(notCriteria);
        }

        // دالة مساعدة لدمج تعبيرين مع استبدال المعاملات
        private static Expression<Func<T, bool>> CombineExpressions<T>(
            Expression<Func<T, bool>> left,
            Expression<Func<T, bool>> right,
            Func<Expression, Expression, Expression> combine)
        {
            var param = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceParameterVisitor(left.Parameters[0], param);
            var rightVisitor = new ReplaceParameterVisitor(right.Parameters[0], param);
            var combinedBody = combine(leftVisitor.Visit(left.Body), rightVisitor.Visit(right.Body));
            return Expression.Lambda<Func<T, bool>>(combinedBody, param);
        }

        private class ReplaceParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression _oldParam;
            private readonly ParameterExpression _newParam;
            public ReplaceParameterVisitor(ParameterExpression oldParam, ParameterExpression newParam)
            {
                _oldParam = oldParam;
                _newParam = newParam;
            }
            protected override Expression VisitParameter(ParameterExpression node)
                => node == _oldParam ? _newParam : base.VisitParameter(node);
        }
    }

    // كلاس داخلي لتجميع المواصفات المدمجة
    internal class CombinedSpecification<T> : BaseSpecification<T>
    {
        public CombinedSpecification(Expression<Func<T, bool>> criteria) : base(criteria) { }
    }
}