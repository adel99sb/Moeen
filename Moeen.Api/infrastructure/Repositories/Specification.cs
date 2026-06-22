using Moeen.Api.Core.Contracts.infrastructure.Repositories;
using Moeen.Api.Core.Entities;
using System.Linq.Expressions;

namespace Moeen.Api.infrastructure.Repositories
{
    public class Specification<T> : ISpecification<T>
        where T : class
    {
        public Expression<Func<T, bool>> Criteria { get; private set; }
        // ✅ legacy includes stay
        public List<Expression<Func<T, object>>> Includes { get; } = new();

        // ✅ new optional include-chain bucket
        public List<Func<IQueryable<T>, IQueryable<T>>> IncludeChains { get; } = new();

        public Expression<Func<T, object>>? OrderBy { get; private set; }
        public Expression<Func<T, object>>? OrderByDescending { get; private set; }
        public int? Skip { get; private set; }
        public int? Take { get; private set; }
        public Expression<Func<Exam, bool>>? Predicate { get; internal set; }

        public Specification(Expression<Func<T, bool>> criteria) => Criteria = criteria;

        // legacy API (unchanged)
        public void AddInclude(Expression<Func<T, object>> include) => Includes.Add(include);

        // new optional API for Include + ThenInclude chains
        public void AddIncludeChain(Func<IQueryable<T>, IQueryable<T>> chain) => IncludeChains.Add(chain);

        public void ApplyOrderBy(Expression<Func<T, object>> orderByExpression) => OrderBy = orderByExpression;
        public void ApplyOrderByDescending(Expression<Func<T, object>> orderByDescExpression) => OrderByDescending = orderByDescExpression;

        public void ApplyPaging(int skip, int take) { Skip = skip; Take = take; }

        public void AddCriteria(Expression<Func<T, bool>> newCriteria)
            => Criteria = Criteria == null ? newCriteria : Criteria.CombineWithAnd(newCriteria);
    }

    public static class Spec
    {
        // legacy helper (unchanged)
        public static Specification<T> For<T>(
            Expression<Func<T, bool>> criteria,
            params Expression<Func<T, object>>[] includes)
            where T : class
        {
            var spec = new Specification<T>(criteria);
            foreach (var inc in includes) spec.AddInclude(inc);
            return spec;
        }

        // NEW helper for chains
        public static Specification<T> ForChain<T>(
            Expression<Func<T, bool>> criteria,
            params Func<IQueryable<T>, IQueryable<T>>[] chains)
            where T : class
        {
            var spec = new Specification<T>(criteria);
            foreach (var c in chains) spec.AddIncludeChain(c);
            return spec;
        }
    }


    public static class ExpressionExtensions
    {
        public static Expression<Func<T, bool>> CombineWithAnd<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            var parameter = Expression.Parameter(typeof(T));

            var leftVisitor = new ReplaceParameterVisitor(first.Parameters[0], parameter);
            var left = leftVisitor.Visit(first.Body);

            var rightVisitor = new ReplaceParameterVisitor(second.Parameters[0], parameter);
            var right = rightVisitor.Visit(second.Body);

            var andExpression = Expression.AndAlso(left, right);

            return Expression.Lambda<Func<T, bool>>(andExpression, parameter);
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
            {
                return node == _oldParam ? _newParam : base.VisitParameter(node);
            }
        }
    }
}
