using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class ProfileConfiguration : IEntityTypeConfiguration<Domain.Entities.Profile>
    {
        public void Configure(EntityTypeBuilder<Domain.Entities.Profile> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.FullName).HasMaxLength(ApplicationConstants.MaxNameLength);
            builder.Property(x => x.Address).HasMaxLength(ApplicationConstants.MaxAddressLength);
            builder.Property(x => x.ProfileImage).HasMaxLength(ApplicationConstants.MaxUrlLength);
            builder.Property(x => x.PhoneNumber).HasMaxLength(ApplicationConstants.MaxPhoneLength);
            builder.HasIndex(x => new { x.UserId, x.FamilyMembership });
        }
    }
}
