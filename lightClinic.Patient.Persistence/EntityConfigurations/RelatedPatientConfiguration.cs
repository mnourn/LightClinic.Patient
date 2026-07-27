using lightClinic.Patient.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace lightClinic.Patient.Persistence.EntityConfigurations
{
    internal class RelatedPatientConfiguration : IEntityTypeConfiguration<RelatedPatient>
    {
        public void Configure(EntityTypeBuilder<RelatedPatient> builder)
        {
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasIndex(x => new { x.ProfileId, x.DoctorId }).IsUnique();
            builder.HasIndex(x => x.DoctorId);
            builder.HasIndex(x => x.ProfileId);

            builder.HasOne(x => x.Profile).WithMany(x => x.RelatedPatients).HasForeignKey(x => x.ProfileId);
        }
    }
}
