using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Common;
using lightClinic.Patient.Persistence.Contexts;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Linq.Expressions;

namespace lightClinic.Patient.Persistence.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        private readonly PatientAuthDbContext _dbContext;
        protected readonly IMemoryCache _memoryCache;
        protected MemoryCacheEntryOptions _cacheOptions { get; set; }
        private const string _allKey = "ALL";

        public GenericRepository(PatientAuthDbContext dbContext, IMemoryCache memoryCache)
        {
            _dbContext = dbContext;
            _memoryCache = memoryCache;

            _cacheOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(ApplicationConstants.AbsoluteExpirationHours),
                SlidingExpiration = TimeSpan.FromMinutes(ApplicationConstants.SlidingExpirationMinutes)
            };
        }

        public IQueryable<T> Entities => _dbContext.Set<T>();

        public async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);

            return entity;
        }

        public Task UpdateAsync(T entity)
        {
            T exist = _dbContext.Set<T>().Find(entity.Id);
            _dbContext.Entry(exist).CurrentValues.SetValues(entity);

            InvalidateCache(entity.Id);

            return Task.CompletedTask;
        }

        public Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);

            InvalidateCache(entity.Id);

            return Task.CompletedTask;
        }

        public virtual async Task<List<T>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cacheKey = $"{typeof(T).Name}_{_allKey}";
            if (!_memoryCache.TryGetValue(cacheKey, out List<T> list))
            {
                list = await Entities.ToListAsync(cancellationToken);
                _memoryCache.Set(cacheKey, list, _cacheOptions);
            }
            return list;
        }

        public async Task<List<T>> FindByAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken)
        {
            return await Entities.Where(condition).ToListAsync(cancellationToken);
        }

        public async Task<T> GetAsync(Expression<Func<T, bool>> condition, CancellationToken cancellationToken)
        {
            return await Entities.FirstOrDefaultAsync(condition, cancellationToken);
        }

        public async Task<IQueryable<T>> GetQueryableAsync()
        {
            return Entities.AsQueryable<T>();
        }

        public void ForceUpdate(T entity)
        {
            _dbContext.Set<T>().Entry(entity).State = EntityState.Modified;
        }

        public async Task<T> GetByIdAsync(long id, CancellationToken cancellationToken)
        {
            var cacheKey = $"{typeof(T).Name}_{id}";
            if (!_memoryCache.TryGetValue(cacheKey, out T? entity))
            {
                entity = await Entities.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
                if (entity is not null)
                {
                    _memoryCache.Set(cacheKey, entity, _cacheOptions);
                }
            }
            return entity;
        }

        private void InvalidateCache(long id)
        {
            var cacheKey = $"{typeof(T).Name}_{id}";
            if (_memoryCache.TryGetValue(cacheKey, out T? entityCached))
            {
                _memoryCache.Remove(cacheKey);
            }
        }
    }
}