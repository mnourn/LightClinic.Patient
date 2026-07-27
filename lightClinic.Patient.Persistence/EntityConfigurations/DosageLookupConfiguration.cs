using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class DosageLookupConfiguration : IEntityTypeConfiguration<DosageLookup>
    {
        public void Configure(EntityTypeBuilder<DosageLookup> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
