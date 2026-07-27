using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class UnitLookupConfiguration : IEntityTypeConfiguration<UnitLookup>
    {
        public void Configure(EntityTypeBuilder<UnitLookup> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
