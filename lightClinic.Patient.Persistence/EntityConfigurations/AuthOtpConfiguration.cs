using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class AuthOtpConfiguration : IEntityTypeConfiguration<AuthOtp>
    {
        public void Configure(EntityTypeBuilder<AuthOtp> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.Email).HasMaxLength(ApplicationConstants.MaxNameLength);
            builder.Property(x => x.PhoneNumber).HasMaxLength(ApplicationConstants.MaxPhoneLength);
        }
    }
}
