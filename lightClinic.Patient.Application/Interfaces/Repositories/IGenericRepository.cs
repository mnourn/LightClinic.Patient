using lightClinic.Patient.Domain.Common.Interfaces;
using System.Linq.Expressions;

namespace lightClinic.Patient.Contracts.Interfaces.Repositories
{
    public interface IGenericRepository<T> where T : class, IEntity
    {
        IQueryable<T> Entities { get; }

        Task<T> GetByIdAsync(long id, CancellationToken cancellationToken);

        Task<List<T>> GetAllAsync(CancellationToken cancellationToken);

        Task<T> AddAsync(T entity);

        Task UpdateAsync(T entity);

        Task DeleteAsync(T entity);
        Task<List<T>> FindByAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken);
        Task<T> GetAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken);
        Task<IQueryable<T>> GetQueryableAsync();

        void ForceUpdate(T entity);
    }
}