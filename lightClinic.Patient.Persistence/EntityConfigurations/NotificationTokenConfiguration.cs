using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class NotificationTokenConfiguration : IEntityTypeConfiguration<NotificationToken>
    {
        public void Configure(EntityTypeBuilder<NotificationToken> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.FCMToken).HasMaxLength(ApplicationConstants.MaxFCMTokenLength);
            builder.Property(x => x.DeviceId).HasMaxLength(ApplicationConstants.MaxFCMTokenLength);
            builder.HasIndex(x => x.UserId);
        }
    }
}
