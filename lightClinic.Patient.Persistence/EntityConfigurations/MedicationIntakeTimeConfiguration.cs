using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations;

internal class MedicationIntakeTimeConfiguration : IEntityTypeConfiguration<MedicationIntakeTime>
{
    public void Configure(EntityTypeBuilder<MedicationIntakeTime> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.MedicationId);

        builder.HasOne(x => x.Medication)
            .WithMany(x => x.IntakeTimes)
            .HasForeignKey(x => x.MedicationId);
    }
}
