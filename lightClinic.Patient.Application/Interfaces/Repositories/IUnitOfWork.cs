using lightClinic.Patient.Domain.Common;
using Microsoft.EntityFrameworkCore.Storage;

namespace lightClinic.Patient.Contracts.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : BaseEntity;

        Task<int> SaveAsync(CancellationToken cancellationToken);

        Task<int> SaveAndRemoveCache(CancellationToken cancellationToken, params string[] cacheKeys);

        Task Rollback();

        Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken);
    }
}