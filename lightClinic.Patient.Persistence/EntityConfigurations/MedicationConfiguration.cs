using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations;

internal class MedicationConfiguration : IEntityTypeConfiguration<Medication>
{
    public void Configure(EntityTypeBuilder<Medication> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.ProfileId);
        builder.HasIndex(x => new { x.ProfileId, x.Status });

        builder.HasOne(x => x.Profile)
            .WithMany(x => x.Medications)
            .HasForeignKey(x => x.ProfileId);

        builder.HasOne(x => x.MedicineLookup)
            .WithMany()
            .HasForeignKey(x => x.MedicineLookupId);

        builder.HasOne(x => x.DosageLookup)
            .WithMany()
            .HasForeignKey(x => x.DosageLookupId);

        builder.HasOne(x => x.UnitLookup)
            .WithMany()
            .HasForeignKey(x => x.UnitLookupId);

        builder.HasOne(x => x.ChronicDiseaseLookup)
            .WithMany()
            .HasForeignKey(x => x.ChronicDiseaseLookupId);
    }
}
