using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class MedicineLookupConfiguration : IEntityTypeConfiguration<MedicineLookup>
    {
        public void Configure(EntityTypeBuilder<MedicineLookup> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
