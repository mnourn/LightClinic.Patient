using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class AddressConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.Location).HasMaxLength(ApplicationConstants.MaxNameLength);
            builder.Property(x => x.BuildingName).HasMaxLength(ApplicationConstants.MaxNameLength).IsRequired();
            builder.Property(x => x.Street).HasMaxLength(ApplicationConstants.MaxNameLength).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(ApplicationConstants.MaxPhoneLength).IsRequired();
            builder.Property(x => x.AdditionalDirections).HasMaxLength(ApplicationConstants.MaxAddressLength);
            builder.HasIndex(x => x.UserId);
        }
    }
}
