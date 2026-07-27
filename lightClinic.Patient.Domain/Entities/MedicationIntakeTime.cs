using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities;

public class MedicationIntakeTime : BaseAuditableEntity
{
    public long MedicationId { get; set; }
    public TimeOnly IntakeTime { get; set; }
    public byte OrderIndex { get; set; }

    // Navigation Property
    public Medication Medication { get; set; }
}
