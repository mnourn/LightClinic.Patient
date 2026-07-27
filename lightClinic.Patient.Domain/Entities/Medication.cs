using lightClinic.Patient.Domain.Common;
using lightClinic.Patient.Shared.Enums;

namespace lightClinic.Patient.Domain.Entities;

public class Medication : BaseAuditableEntity
{
    public Guid ProfileId { get; set; }
    public long? BookingId { get; set; }
    public int MedicineLookupId { get; set; }
    public int DosageLookupId { get; set; }
    public int UnitLookupId { get; set; }
    public MedicationFrequency Frequency { get; set; }
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public int? ChronicDiseaseLookupId { get; set; }
    public MedicineCategory? Category { get; set; }
    public MedicationStatus Status { get; set; }

    // Navigation Properties
    public Profile Profile { get; set; }
    public MedicineLookup MedicineLookup { get; set; }
    public DosageLookup DosageLookup { get; set; }
    public UnitLookup UnitLookup { get; set; }
    public ChronicDisease ChronicDiseaseLookup { get; set; }

    // Related Collections
    public virtual ICollection<MedicationIntakeTime> IntakeTimes { get; set; }
    public virtual ICollection<MedicationReminder> Reminders { get; set; }
}

