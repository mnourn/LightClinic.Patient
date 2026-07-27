using lightClinic.Patient.Contracts.Interfaces.Repositories;
using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace lightClinic.Patient.Persistence.Repositories;

public class GovernmentRepository : GenericRepository<Government>, IGovernmentRepository
{
    private const string _cacheKey = $"{nameof(Government)}_ALL";

    public GovernmentRepository(
        PatientAuthDbContext dbContext,
        IMemoryCache memoryCache) : base(dbContext, memoryCache) { }

    public override async Task<List<Government>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (!_memoryCache.TryGetValue(_cacheKey, out List<Government> list))
        {
            list = await Entities.Include(x => x.Cities).ToListAsync(cancellationToken);
            _memoryCache.Set(_cacheKey, list, _cacheOptions);
        }
        return list;
    }
}
