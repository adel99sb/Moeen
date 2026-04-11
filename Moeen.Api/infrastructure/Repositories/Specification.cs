using Moeen.Api.Core.Specifications;
using System.Linq.Expressions;

namespace Moeen.Api.infrastructure.Repositories
{
    /// <summary>
    /// مواصفة بسيطة جاهزة للاستخدام السريع.
    /// </summary>
    public sealed class Specification<T> : BaseSpecification<T>, ISpecification
    {
        public Specification() : base() { }

        public Specification(Expression<Func<T, bool>> criteria) : base(criteria) { }
    }
}
