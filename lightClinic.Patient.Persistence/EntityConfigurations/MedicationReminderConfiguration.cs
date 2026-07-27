using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations;

internal class MedicationReminderConfiguration : IEntityTypeConfiguration<MedicationReminder>
{
    public void Configure(EntityTypeBuilder<MedicationReminder> builder)
    {
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.HasIndex(x => x.MedicationId);
        builder.HasIndex(x => new { x.ScheduledDate, x.ScheduledTime, x.IsTaken });

        builder.HasOne(x => x.Medication)
            .WithMany(x => x.Reminders)
            .HasForeignKey(x => x.MedicationId);
    }
}
