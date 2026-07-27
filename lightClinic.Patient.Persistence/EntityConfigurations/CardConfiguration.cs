using lightClinic.Patient.Domain.Entities;
using lightClinic.Patient.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class CardConfiguration : IEntityTypeConfiguration<Card>
    {
        public void Configure(EntityTypeBuilder<Card> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Property(x => x.CardHolderName).HasMaxLength(ApplicationConstants.MaxNameLength).IsRequired();
            builder.HasIndex(x => x.UserId);
        }
    }
}
