namespace Moeen.Api.Core.Contracts.infrastructure.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid Id);
        Task<IEnumerable<T>> GetAllAsync(ISpecification<T> specification = null);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task SaveAsync();
        IQueryable<T> GetAllQueryable();
    }
}