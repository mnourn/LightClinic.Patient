using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class UserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.RefreshToken).HasMaxLength(ApplicationConstants.RefreshTokenLength);
            builder.Property(x => x.PhoneNumber).HasMaxLength(ApplicationConstants.MaxPhoneLength);
            builder.Ignore(x => x.TwoFactorEnabled);
            builder.HasMany(x => x.Profiles).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Addresses).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Cards).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.NotificationTokens).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasMany(x => x.Notifications).WithOne(x => x.User).HasForeignKey(x => x.UserId);
            builder.HasIndex(x => x.Email).IsUnique().HasFilter("[Email] IS NOT NULL");
            builder.HasIndex(x => x.PhoneNumber).IsUnique();
        }
    }
}
