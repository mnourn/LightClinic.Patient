using lightClinic.Patient.Domain.Common;

namespace lightClinic.Patient.Domain.Entities;

public class MedicationReminder : BaseAuditableEntity
{
    public long MedicationId { get; set; }
    public DateOnly ScheduledDate { get; set; }
    public TimeOnly ScheduledTime { get; set; }
    public bool IsTaken { get; set; }
    public DateTime? TakenAt { get; set; }
    public bool IsMissed { get; set; }
    public DateTime? ReminderSentAt { get; set; }
    public DateTime? SnoozedUntil { get; set; } // When the reminder should be shown again after snoozing

    // Navigation Property
    public Medication Medication { get; set; }
}
