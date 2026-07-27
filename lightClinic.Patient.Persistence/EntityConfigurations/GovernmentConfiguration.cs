using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class GovernmentConfiguration : IEntityTypeConfiguration<Government>
    {
        public void Configure(EntityTypeBuilder<Government> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.Name).HasMaxLength(ApplicationConstants.MaxNameLength).IsRequired();
            builder.HasMany(x => x.Cities).WithOne(x => x.Government).HasForeignKey(x => x.GovernmentId);
        }
    }
}
